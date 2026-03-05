using Microsoft.EntityFrameworkCore;
using Robi_App.Data;
using Robi_App.Models;
using Robi_App.Models.ViewModels;

namespace Robi_App.Services.Implementation
{
    public class GiftService : IGiftService
    {
        private readonly ApplicationDbContext _db;

        public GiftService(ApplicationDbContext db)
        {
            _db = db;
        }
        public void CreateGift(CreateGiftVM gift)
        {
            _db.Add(new Gift
            {
                Name = gift.Name,   
                ImagePath = gift.ImagePath, 
            }); 
            _db.SaveChanges();  
        }

        public bool DeleteGift(int id)
        {
            var gift = _db.Gifts.FirstOrDefault(g => g.Id == id); 
            if (gift == null)
                return false;

            _db.Gifts.Remove(gift);
            _db.SaveChanges();
            return true;
        }

        public IEnumerable<ShowGiftVM> GetAll()
        {
            return _db.Gifts.AsNoTracking().Select(g => new ShowGiftVM
            {
                Name = g.Name,
                ImagePath = g.ImagePath,
            }).ToList(); 
        }
    }
}
