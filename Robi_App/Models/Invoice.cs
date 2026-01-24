using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Robi_App.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public string UserId { get; set; } = null!;
        public bool IsReviewed { get; set; }
        // this code : the id for the main Invoice 
        //ask for code len 
        [StringLength(25)]
        public string Code { get; set; } = null!;
        public int Points { get; set; } 
        public DateOnly CreatedAt { get; set; } = new DateOnly(DateTime.Now.Year , DateTime.Now.Month , DateTime.Now.Day); 
        [ValidateNever]
        [ForeignKey("StoreId")]
        public Store Store { get; set; } = null!;
        [ValidateNever]
        [ForeignKey("UserId")]
        public IdentityUser User { get; set; } = null!; 

    }

}
