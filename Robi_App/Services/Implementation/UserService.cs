using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<EmployeeVM>> GetAllEmployees()
        {
            var employees = await _userManager.GetUsersForClaimAsync
                (new Claim(SD.Role_Employee, SD.Role_Employee));

            return employees.Select(u => new EmployeeVM
            {
                EmployeeId = u.Id,
                FullName = u.FullName,
                PhoneNumber = u.UserName!,
                PassWord = u.TemporaryPassword,
            }).ToList();
        }

        public async Task<EmployeeVM> GetEmployee(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return null!;

            var emp  =  new EmployeeVM
            {
                EmployeeId = user.Id,
                FullName = user.FullName,
                IsLocked = user.LockoutEnd > DateTime.UtcNow ? true : false,
                PhoneNumber = user.UserName!
            };

            var claims = await _userManager.GetClaimsAsync(user);
            var storeId = claims.Where(c => c.Type == SD.ForStore).Select(C=> C.Value).FirstOrDefault();


            if (int.TryParse(storeId, out int parsedStoreId))
            {
                emp.StoreName = _db.Stores.Where(s => s.Id == parsedStoreId)
                    .Select(s => s.Title).FirstOrDefault()!; 
            }

            return emp; 
           
        }
    }
}
