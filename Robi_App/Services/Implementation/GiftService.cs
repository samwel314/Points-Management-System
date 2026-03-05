using Microsoft.EntityFrameworkCore;
using Robi_App.Data;
using Robi_App.Models;
using Robi_App.Models.ViewModels;

namespace Robi_App.Services.Implementation
{
    public class GiftService : IGiftService
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _environment;

        public GiftService(ApplicationDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
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
            var path = Path.Combine(_environment.WebRootPath, "GiftsImages", gift.ImagePath);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            _db.Gifts.Remove(gift);
            _db.SaveChanges();
            return true;
        }

        public IEnumerable<ShowGiftVM> GetAll()
        {
            return _db.Gifts.AsNoTracking().Select(g => new ShowGiftVM
            {
                Id = g.Id,  
                Name = g.Name,
                ImagePath = g.ImagePath,
            }).ToList(); 
        }

        public bool HaveGiftWithName(string name)
        {
            return _db.Gifts.Any(g => g.Name == name);
        }
    }
}
