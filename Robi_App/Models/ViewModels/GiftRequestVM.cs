namespace Robi_App.Models.ViewModels
{
    public class GiftRequestVM
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string GiftName { get; set; } = null!;
        public int GiftPoints { get; set; }  
        public string Notes { get; set; } = null!;  
        public DateOnly RequestDate { get; set; }
        public bool IsApproved { get; set; }
    }
}
