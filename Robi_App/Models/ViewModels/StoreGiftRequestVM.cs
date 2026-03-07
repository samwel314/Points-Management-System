namespace Robi_App.Models.ViewModels
{
    public class StoreGiftRequestVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public IEnumerable<GiftRequestVM> giftRequests { get; set; } = null!; 
    }
}
