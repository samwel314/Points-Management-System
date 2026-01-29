using Microsoft.EntityFrameworkCore;
using Robi_App.Data;
using Robi_App.Models;
using Robi_App.Models.ViewModels;

namespace Robi_App.Services.Implementation
{
    public class StoreService : IStoreService
    {
        private readonly ApplicationDbContext _db;
        public StoreService(ApplicationDbContext db)
        {
            _db = db;
        }

        public void AddStore(Store store)
        {
            _db.Stores.Add(store);   
            _db.SaveChanges();  
        }

        public void DeleteStore(Store store)
        {
            _db.Stores.Remove(store);
            _db.SaveChanges();  
        }

        public Store ? GetStoreById(int id , bool tracking)
        {
            if(!tracking)
                return _db.Stores.AsNoTracking().FirstOrDefault(s=>s.Id == id);    
            return _db.Stores.Find(id); 
        }

        public IEnumerable<Store> GetStores(bool tracking)
        {
            var query = _db.Stores.AsQueryable();
            if (!tracking)
            {
                query = query.AsNoTracking();
            }
            return query.ToList();  
        }

        public bool StoreExists(int id)
        {
            return _db.Stores.Any(s => s.Id == id); 
        }

        public void UpdateStore(Store store)
        {
            _db.Stores.Update(store);           
            _db.SaveChanges();  
        }
        public ShowStoreInvoicesVM ShowStoreInvoicesVM(int id)
        {
            var store = _db.Stores.Find(id);
            if (store == null)
                return null!;
            var firstDayOfMonth = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1);
            var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);
            return new ShowStoreInvoicesVM()
            {
                Id = id,    
                StoreName = store.Title , 
                TotalInvoices = _db.Invoices.Where(i=>i.StoreId == id).Count(),
                TotalToDayInvoices = _db.Invoices.
                Where(i => i.StoreId == id && i.CreatedAt == DateOnly.FromDateTime(DateTime.Now)).Count(),
                TotalToMonthInvoices = _db.Invoices.Where(i => i.StoreId == id && i.CreatedAt >= firstDayOfMonth && i.CreatedAt < firstDayOfNextMonth ).Count(),
            }; 
        }
 
    }
}
