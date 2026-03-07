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

        public void CreateInvoice(CreateInvoiceVM model)
        {
            var invoiceToDB = new Invoice()
            {
                Code = model.Code,
                UserId = model.UserId,  
                StoreId = model.StoreId,    
                ImagePath = model.ImagePath!
            }; 
            _db.Invoices.Add(invoiceToDB);      
            _db.SaveChanges();      
        }

        public CustomerProfileVM GetCustomerProfile(string Id)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Id == Id);
            if (user == null)
                return null!;

            var data = new CustomerProfileVM()
            {
                CustomerId = Id,
                CustomerName = user.FullName , 
                PhoneNumber = user.UserName! ,
                IsLocked = user.LockoutEnd > DateTime.UtcNow ? true : false,
                Invoices = _db.Invoices.Where(i => i.UserId == Id).Select(i => new ShowInvoiceVM
                {
                    Points = i.Points,
                    Id = i.Id,
                    code = i.Code,
                    storeName = i.Store.Title,
                    Date = i.CreatedAt.ToString("dd/MM/yyyy" ) , 
                    ImagePath = i.ImagePath
                }).ToList(),
                TotalPoints = _db.Invoices.Where(i => i.UserId == Id).Sum(i => i.Points)
            };
            return data;
        }

        public string ? GetImagePath(int id , string userId )
        {
            Invoice ? invoice;
            if (userId != null)
                invoice = _db.Invoices.FirstOrDefault(i => i.Id == id && i.UserId == userId);
            else
                invoice = invoice = _db.Invoices.FirstOrDefault(i => i.Id == id); 
            return invoice is not null ? invoice.ImagePath : null;
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

        public  async Task RestYear()
        {
            _db.Invoices.RemoveRange(_db.Invoices);
            _db.GiftRequests.RemoveRange(_db.GiftRequests); 
          await  _db.SaveChangesAsync();  
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
                var storeId = int.Parse(user.FindFirst(c => c.Type == SD.ForStore)!.Value);
                query = query.Where(c
               => c.StoreId == storeId);
            }
            if (filter != null)
                query = query.Where(filter);   
            
            return query.Select (x => new ShowInvoiceVM { 
            Id = x.Id,  
            code=x.Code,    
            ImagePath = x.ImagePath,    
            storeName =x.Store.Title,   
            UserName = _db.ApplicationUsers.First(u => u.Id == x.UserId)!.FullName,
            Phone = x.User.UserName!,
            Points = x.Points,
            Date = x.CreatedAt.ToString("dd/MM/yyyy")
            } ).ToList();
        }

        public bool UpdateImage(int id, string imagePath)
        {
            var invoice = _db.Invoices.FirstOrDefault(i => i.Id == id);
            if (invoice == null)
                return false;
            invoice.ImagePath = imagePath;  
            _db.SaveChanges();  
            return true;
        }
    }
}
