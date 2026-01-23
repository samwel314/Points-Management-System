using Microsoft.EntityFrameworkCore;
using Robi_App.Data;
using Robi_App.Models;

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
    }
}
