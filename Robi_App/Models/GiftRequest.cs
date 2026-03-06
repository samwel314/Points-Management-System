using System.ComponentModel.DataAnnotations.Schema;

namespace Robi_App.Models
{
    public class GiftRequest 
    {
        public int Id { get; set; } 
        public int GiftId { get; set; }
        public int StoreId { get; set; }
        public string? Notes { get; set; }
        public string UserId { get; set; } = null!;
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.Now);  
        public bool IsApproved { get; set; } = false;

        // nva
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;
        [ForeignKey("GiftId")]
        public Gift Gift { get; set; } = null!; 
        [ForeignKey("StoreId")]
        public Store Store { get; set; } = null!;
    }

}
