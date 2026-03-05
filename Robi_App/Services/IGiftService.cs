using Robi_App.Models.ViewModels;

namespace Robi_App.Services
{
    public interface IGiftService
    {
        void CreateGift(CreateGiftVM gift);
        IEnumerable<ShowGiftVM> GetAll(); 
        bool DeleteGift(int id);    
        bool HaveGiftWithName(string name);
        Task<bool> UpdateImage(int id  , IFormFile image );
    }
}
