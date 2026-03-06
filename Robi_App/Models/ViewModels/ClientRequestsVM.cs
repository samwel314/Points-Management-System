namespace Robi_App.Models.ViewModels
{
    public class ClientRequestsVM
    {
        public int Id { get; set; }
        public string GiftName { get; set; } = null!;
        public int Points { get; set; } 
        public string StoreName { get; set; } = null!;
        public string Notes { get; set; } = null!;
        public DateOnly CreatedAt { get; set; }
        public bool IsApproved { get; set; }
    }
}
