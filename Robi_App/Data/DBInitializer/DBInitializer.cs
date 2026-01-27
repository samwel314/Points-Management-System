using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Robi_App.Data.DBInitializer
{
    public class DBInitializer : IDBInitializer
    {

        UserManager<IdentityUser> _usermanager;
        ApplicationDbContext _db;
        public DBInitializer(UserManager<IdentityUser> _usermanager , 
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
                _db.Users.FirstOrDefault(u => u.UserName ==
                "01066389260");
            if (appUser is null)
            {
                _usermanager.CreateAsync(new IdentityUser
                {
                    UserName = "01066389260",
                }, "R@Glc123").GetAwaiter().GetResult();

                appUser =
                   _db.Users.FirstOrDefault(u => u.UserName ==
                   "01066389260");

                _usermanager.AddClaimsAsync(appUser!,
                   new List<Claim>(){
                       new Claim(SD.UserName, "Romany Hany") ,
                       new Claim(SD.Role_Admin , SD.Role_Admin) ,
                   }).GetAwaiter().GetResult(); 
            }

            return;
        }
    }
}
