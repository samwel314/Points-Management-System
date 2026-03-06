namespace Robi_App.Models.ViewModels
{
    public class RequestGiftViewModel
    {
        public int GiftId { get; set; }
        public int BranchId { get; set; }
        public string? Notes { get; set; }
        public string UserId { get; set; } = null!;     
    }
}
