using Microsoft.AspNetCore.Mvc.Rendering;

namespace Robi_App
{
    public static class SD
    {
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";
        public const string Role_Client = "Client";
        public const string UserName = "Name";  
        public const string ForStore = "StoreId";
        public static IEnumerable <SelectListItem> 
            Roles =  new List <SelectListItem>()
            {
                new SelectListItem
                {
                    Text = "Admin",
                    Value = Role_Admin,
                } ,
                 new SelectListItem
                {
                    Text = "Client",
                    Value = Role_Client,
                } ,
                  new SelectListItem
                {
                    Text = "Employee",
                    Value = Role_Employee,
                } ,
            };  
    }
}
