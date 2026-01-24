using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Robi_App.Models.ViewModels
{
    public class CreateUpdateInvoiceVM
    {
        // invoice id
        public int Id { get; set; }
        public int StoreId { get; set; }    
        [Display(Name = "Invoice Number")]
        [Required(ErrorMessage = "Enter Invoice Number")]
        [MinLength(7)]
        [MaxLength(25)]
        public string Code { get; set; } = null!;
        [ValidateNever] // get it from current user 
        public string UserId { get; set; } = null!;
        [ValidateNever]
        public IEnumerable< SelectListItem>  Stores  { get; set; } = null!;      
    }
}
