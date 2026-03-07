using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Robi_App.Data;
using Robi_App.Models;
using Robi_App.Models.ViewModels;
using System.Net.WebSockets;

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

        public async Task<bool> AddGiftRequest(RequestGiftViewModel model)
        {
            if (!await _db.Stores.AnyAsync(s => s.Id == model.BranchId) ||
                       !await _db.Gifts.AnyAsync(g => g.Id == model.GiftId))
                return false;

            _db.GiftRequests.Add(new GiftRequest
            {
                GiftId = model.GiftId,
                StoreId = model.BranchId,
                Notes = model.Notes,
                UserId = model.UserId,
            });
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<int> AvailablePoints(string userId)
        {
            var total = await _db.Invoices.Where(i => i.UserId == userId).SumAsync(i => i.Points);
            var used = await _db.GiftRequests.Where(gr => gr.UserId == userId).SumAsync(gr => gr.Gift.Points);
            return total - used;    
        }

        public async Task<bool> ChangeAvailability(int id)
        {
            var gift = await _db.GiftRequests.FindAsync(id);
            if (gift == null) return false;

            gift.IsApproved = !gift.IsApproved;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ClientRequestsVM>> ClientRequstedGifts(string userId)
        {
            return await _db.GiftRequests.Where(gr => gr.UserId == userId).
                Select(gr =>  new ClientRequestsVM
                {
                    Id = gr.Id, 
                    GiftName = gr.Gift.Name,    
                    Points  = gr.Gift.Points,   
                    StoreName = gr.Store.Title,  
                    CreatedAt = gr.CreatedAt,       
                    IsApproved = gr.IsApproved,     
                }) .ToListAsync();   
        }

        public void CreateGift(CreateGiftVM gift)
        {
            _db.Add(new Gift
            {
                Name = gift.Name,
                ImagePath = gift.ImagePath,
                Points = gift.Points,
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

        public async Task<bool> DeleteGiftRequest(int id, string userIid = null)
        {
            var giftRequest = await _db.GiftRequests.
                FirstOrDefaultAsync(gr => gr.Id == id );
            if (giftRequest == null)
                return false; 
            if (!string.IsNullOrEmpty(userIid) && giftRequest.UserId != userIid)
                return false;
            _db.GiftRequests.Remove(giftRequest);
           await _db.SaveChangesAsync(); 
            return true;
        }

        public IEnumerable<ShowGiftVM> GetAll()
        {
            return _db.Gifts.AsNoTracking().Select(g => new ShowGiftVM
            {
                Id = g.Id,
                Name = g.Name,
                ImagePath = g.ImagePath,
                points = g.Points,
            }).ToList();
        }

        public int GetGiftPoints(int giftId)
        {
            var gift = _db.Gifts.FirstOrDefault(g => g.Id == giftId);
            return gift == null ? 0 : gift.Points;
        }

        public bool HaveGiftWithName(string name)
        {
            return _db.Gifts.Any(g => g.Name == name);
        }

        public Task<bool> IsReqestedBefore(int giftId, string userId)
        {
            return _db.GiftRequests.AnyAsync(gr => gr.GiftId == giftId && gr.UserId == userId); 
        }

        public bool UpdataName(int id, string name)
        {

            var gift = _db.Gifts.FirstOrDefault(g => g.Id == id);
            if (gift == null)
                return false;
            if (gift.Name != name && !HaveGiftWithName(name))
            {
                gift.Name = name;
                _db.SaveChanges();
                return true;
            }
            return true;
        }

        public bool UpdataPoints(int id, int points)
        {

            var gift = _db.Gifts.FirstOrDefault(g => g.Id == id);
            if (gift == null)
                return false;
            gift.Points = points;
            _db.SaveChanges();
            return true;

        }

        public async Task<bool> UpdateImage(int id, IFormFile image)
        {
            var gift = _db.Gifts.FirstOrDefault(g => g.Id == id);
            if (gift == null)
                return false;
            // remove old image 
            var path = Path.Combine(_environment.WebRootPath, "GiftsImages", gift.ImagePath);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            // add new image
            path = Path.Combine(_environment.WebRootPath, "GiftsImages");

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(path, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(stream);
            gift.ImagePath = fileName;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
