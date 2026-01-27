using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Robi_App.Models;

namespace Robi_App.AuthorizationRequirement.Handler
{
    public class InvoiceOwnerhandler : AuthorizationHandler 
        <IsInvoiceOwnerRequirement, Invoice>
    {
        protected readonly UserManager<IdentityUser> _userManager;

        public InvoiceOwnerhandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsInvoiceOwnerRequirement requirement, Invoice resource)
        {
            var user = await _userManager.GetUserAsync(context.User); 
            if (user == null) 
                return;
            if (user.Id == resource.UserId)
            {
                context.Succeed(requirement); 
            }
                
        }
    }
}
