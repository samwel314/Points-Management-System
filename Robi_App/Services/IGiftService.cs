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

        bool UpdataName(int id, string name);
        bool UpdataPoints(int id, int points);

        Task<bool> AddGiftRequest(RequestGiftViewModel model); 
        Task<bool> IsReqestedBefore(int giftId, string userId);
        Task<IEnumerable<ClientRequestsVM>> ClientRequstedGifts(string userId);
        Task<bool> DeleteGiftRequest(int id, string userIid  = null! );

        Task<int> AvailablePoints(string userId); 
        int GetGiftPoints(int giftId);
        Task<bool> ChangeAvailability(int id);
    }
}
