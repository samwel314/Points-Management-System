using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Robi_App.Models.ViewModels
{
    public class CreateGiftVM
    {
        public string Name { get; set; } = null!;
        public IFormFile Image { get; set; } = null!;
        [ValidateNever]
        public string ImagePath { get; set; } = null!;
    }
}
