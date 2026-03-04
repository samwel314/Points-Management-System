// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Robi_App.Models;
using Robi_App.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace Robi_App.Areas.Identity.Pages.Account
{
    //[Authorize (Policy = SD.Role_Admin)]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IStoreService _storeService;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender
            , IStoreService storeService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _storeService = storeService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>

            [Display(Name = "Full Name")]
            [Required(ErrorMessage = "ادخل الاسم بالكامل  ")]
            public string FullName { get; set; }
            [RegularExpression(@"^01[0-2,5]{1}[0-9]{8}$",
              ErrorMessage = "هذ الرقم غير صحيح  ")]
            [Required(ErrorMessage = "ادخل رقم الهاتف")]
            public string PhoneNumber { get; set; }
            [ValidateNever]
            public string Role { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "ادخل كلمة السر ")]
            [StringLength(20, MinimumLength = 3,
    ErrorMessage = "{0} يجب أن يكون على الأقل {2} أحرف وبحد أقصى {1} أحرف.")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "كلمة المرور وتأكيدها غير متطابقين.")]
            public string ConfirmPassword { get; set; }
            public int StoreId { get; set; }

            public IEnumerable<SelectListItem> Stores { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            if (User.HasClaim(C => C.Type == SD.Role_Admin))
            {
                Input = new InputModel()
                {
                    Stores = _storeService.GetStores(false)
                    .Select(st => new SelectListItem
                    {
                        Text = st.Title,
                        Value = st.Id.ToString()
                    }).ToList()
                };

            }
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.PhoneNumber, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, "", CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    user.FullName = Input.FullName;
                    var claims = new List<Claim>(); 

                    if (User.HasClaim(C => C.Type == SD.Role_Admin))
                    {
                        if (Input.Role == null)
                        {
                            ModelState.AddModelError("Role", "اختار نوع الحساب"); 
                            return Page();
                        }
                        claims.Add(new Claim(Input.Role, Input.Role));                        

                        if (Input.StoreId != 0)
                            claims.Add(new Claim(SD.ForStore, Input.StoreId.ToString()));
                    }
                    else
                    {
                        /// this client register with herself 
                        claims.Add(new Claim(SD.Role_Client, SD.Role_Client));
                        user.LockoutEnd = DateTime.Now.AddYears(100);
                        returnUrl = "/Home/RegisterSuccess";
                    }

                    if (Input.Role != SD.Role_Admin)
                        user.TemporaryPassword = Input.Password;

                    await _userManager.UpdateAsync(user);
                    await _userManager.AddClaimsAsync(user, claims);
                    _logger.LogInformation("User created a new account with password.");


                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync("app@Gmail.com", "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = "app@Gmail.com", returnUrl = returnUrl });
                    }
                    else
                    {
                        //  await _signInManager.SignInAsync(user, isPersistent: false);

                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            if (User.HasClaim(C => C.Type == SD.Role_Admin))
            {
                Input.Stores = _storeService.GetStores(false)
                .Select(st => new SelectListItem
                {
                    Text = st.Title,
                    Value = st.Id.ToString()
                }).ToList();
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
