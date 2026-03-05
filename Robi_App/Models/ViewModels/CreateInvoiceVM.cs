using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace Robi_App.Models.ViewModels
{
    public class CreateInvoiceVM
    {
        // invoice id
        public int Id { get; set; }
        public int StoreId { get; set; }    
        [Display(Name = "Invoice Number")]
        [Required(ErrorMessage = "ادخل كود الفاتورة ")]
        [MinLength(1)]
        [MaxLength(25)]
        public string Code { get; set; } = null!;
        [ValidateNever] // get it from current user 
        public string UserId { get; set; } = null!;
        [ValidateNever]
        public IEnumerable< SelectListItem>  Stores  { get; set; } = null!;
        [Display(Name = "صورة الفاتورة")]
        [Required (ErrorMessage = "ارفق صورة الفاتورة ")]
        public IFormFile Image { get; set; } = null!;
    }
}
