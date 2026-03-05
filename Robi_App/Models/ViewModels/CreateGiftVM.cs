using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Robi_App.Models.ViewModels
{
    public class CreateGiftVM
    {
        [Display (Name = " اسم الهدية ")]
        [Required (ErrorMessage = "ادخل اسم الهدية")]
        public string Name { get; set; } = null!;
        [Display(Name = " صورة الهدية ")]
        [Required(ErrorMessage = "ارفق صورة الهدية")]
        public IFormFile Image { get; set; } = null!;
        [ValidateNever]
        public string ImagePath { get; set; } = null!;
        [Display (Name = "النقط المستحقة ")]
        [Required (ErrorMessage = "ادخل عدد النقط المستحقة ")]
        [Range(minimum: 0 , maximum: int.MaxValue , ErrorMessage = "لا يمكن ادخال رقم اقل من صفر ")]
        public int Points { get; set; }

    }
}
