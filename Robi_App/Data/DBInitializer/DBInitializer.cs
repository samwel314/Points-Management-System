using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Robi_App.Models;
using System.Security.Claims;

namespace Robi_App.Data.DBInitializer
{
    public class DBInitializer : IDBInitializer
    {

        UserManager<ApplicationUser> _usermanager;
        ApplicationDbContext _db;
        public DBInitializer(UserManager<ApplicationUser> _usermanager , 
        ApplicationDbContext _db)
        {
            this._usermanager = _usermanager;
            this._db = _db;
        }
        public void Initialize()
        {

            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }

            }
            catch (Exception e)
            {

            }
            var appUser =
                _db.ApplicationUsers.FirstOrDefault(u => u.UserName ==
                "01005415303");
            if (appUser is null)
            {
                _usermanager.CreateAsync(new ApplicationUser
                {
                    UserName = "01005415303",
                    FullName = "Demo Admin"
                }, "123456").GetAwaiter().GetResult();

                appUser =
                   _db.ApplicationUsers.FirstOrDefault(u => u.UserName ==
                   "01005415303");

                _usermanager.AddClaimAsync(appUser!,
                       new Claim(SD.Role_Admin , SD.Role_Admin) 
                   ).GetAwaiter().GetResult(); 
            }

            return;
        }
    }
}
