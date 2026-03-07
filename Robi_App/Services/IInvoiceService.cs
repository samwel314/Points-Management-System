using Microsoft.EntityFrameworkCore.ChangeTracking;
using Robi_App.Models;
using Robi_App.Models.ViewModels;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Robi_App.Services
{
    public interface IInvoiceService
    {
        public string ?GetImagePath (int id , string  userId);
        public bool UpdateImage (int id, string imagePath); 
        void CreateInvoice(CreateInvoiceVM model); 
        void Save();
        bool IsCodeExisting(string code);
        Invoice ? GetInvoiceToUpdate(int Id , bool tracked); 

        public CustomerProfileVM GetCustomerProfile(string Id);

        public IEnumerable<ShowInvoiceVM> showInvoices(ClaimsPrincipal user , Expression<Func<Invoice , bool > > filter = null!);
        
        public Task RestYear(); 
    }
}
