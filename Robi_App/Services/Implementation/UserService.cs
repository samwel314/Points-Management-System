using Microsoft.AspNetCore.Identity;
using Robi_App.Data;
using Robi_App.Models;
using Robi_App.Models.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Robi_App.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IEnumerable<CustomerProfileVM>> GetAllCustomers()
        {
            var Customers = await _userManager.GetUsersForClaimAsync(new Claim(SD.Role_Client, SD.Role_Client));
             return Customers.Select(u => new CustomerProfileVM { 
                CustomerId = u.Id,  
                CustomerName = u.FullName,
                PhoneNumber = u.UserName! , 
                PassWord = u.TemporaryPassword, 
             }).ToList();  
        }
    }
}
