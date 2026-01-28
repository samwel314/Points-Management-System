using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Robi_App
{
    public static class SD
    {
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";
        public const string Role_Client = "Client";
        public const string UserName = "Name";
        public const string UserPassword = "Password";
        public const string ForStore = "StoreId";
        public const string zeroPoints = "zeroPoints";
        public const string hasPoints = "hasPoints"; 
        public static IEnumerable<SelectListItem>
            Roles = new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "مدير ",
                    Value = Role_Admin,
                } ,
                 new SelectListItem
                {
                    Text = "عميل",
                    Value = Role_Client,
                } ,
                  new SelectListItem
                {
                    Text = "موظف",
                    Value = Role_Employee,
                } ,
            };

    }
    
}

