using Microsoft.EntityFrameworkCore.ChangeTracking;
using Robi_App.Models;
using Robi_App.Models.ViewModels;
using System.Linq.Expressions;

namespace Robi_App.Services
{
    public interface IInvoiceService
    {
        void CreateInvoice(CreateUpdateInvoiceVM model); 
        void Save();
        bool IsCodeExisting(string code);
        Invoice ? GetInvoiceToUpdate(int Id , bool tracked); 

        public CustomerProfileVM GetCustomerProfile(string Id);

        public IEnumerable<ShowInvoiceVM> showInvoices(Expression<Func<Invoice , bool > > filter = null!); 
    }
}
