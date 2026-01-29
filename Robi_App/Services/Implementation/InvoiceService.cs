using Microsoft.EntityFrameworkCore;
using Robi_App.Data;
using Robi_App.Models;
using Robi_App.Models.ViewModels;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Robi_App.Services.Implementation
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _db;
        
        public InvoiceService(ApplicationDbContext db)
        {
            _db = db;
        }

        public void CreateInvoice(CreateUpdateInvoiceVM model)
        {
            var invoiceToDB = new Invoice()
            {
                Code = model.Code,
                UserId = model.UserId,  
                StoreId = model.StoreId,    
            }; 
            _db.Invoices.Add(invoiceToDB);      
            _db.SaveChanges();      
        }

        public CustomerProfileVM GetCustomerProfile(string Id)
        {
            var user = _db.ApplicationUsers.First(u => u.Id == Id);
            if (user == null)
                return null!;

            var data = new CustomerProfileVM()
            {
                CustomerId = Id,
                CustomerName = user.FullName , 
                PhoneNumber = user.UserName! ,
                Invoices = _db.Invoices.Where(i => i.UserId == Id).Select(i => new ShowInvoiceVM
                {
                    Points = i.Points,
                    Id = i.Id,
                    code = i.Code,
                    storeName = i.Store.Title,
                    Date = i.CreatedAt.ToString("dd/MM/yyyy" ) , 
                }).ToList(),
                TotalPoints = _db.Invoices.Where(i => i.UserId == Id).Sum(i => i.Points)
            };
            return data;
        }

        public Invoice ? GetInvoiceToUpdate(int Id ,  bool tracked)
        {
            var query = _db.Invoices.AsQueryable(); 
            if (!tracked)
                query = query.AsNoTracking();   
           return query.Where(inv=> inv.Id == Id)    
            .FirstOrDefault();
        }

        public bool IsCodeExisting(string code)
        {
            return _db.Invoices.Any(x => x.Code == code);   
        }

        public void Save()
        {
            _db.SaveChanges();  
        }

        public IEnumerable<ShowInvoiceVM> showInvoices(ClaimsPrincipal user ,Expression<Func<Invoice, bool>> filter = null!)
        {

            var query = _db.Invoices.AsNoTracking().AsQueryable();

            if (user is not null &&  user.HasClaim(c => c.Type == SD.Role_Employee))
            {
                var storeId = int.Parse(user.FindFirst(c => c.Type == SD.Role_Employee)!.Value);
                query = query.Where(c
               => c.StoreId == storeId);
            }
            if (filter != null)
                query = query.Where(filter);   
            
            return query.Select (x => new ShowInvoiceVM { 
            Id = x.Id,  
            code=x.Code,    
            storeName =x.Store.Title,   
            UserName = _db.ApplicationUsers.First(u => u.Id == x.UserId)!.FullName,
            Phone = x.User.UserName!,
            Points = x.Points,
            Date = x.CreatedAt.ToString("dd/MM/yyyy")
            } ).ToList();
        }
    }
}
