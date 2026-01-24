using System.ComponentModel.DataAnnotations;

namespace Robi_App.Models
{
    public class Store
    {
        public int Id { get; set;  }
        [Required(ErrorMessage = "Please Enter Valid Value")]
        [MinLength(5 , ErrorMessage ="Store Title Must Be Great Than 4 Chars")]
        [MaxLength(71, ErrorMessage = "Store Title Must Be Less Than 70 Chars")]
        public string Title { get; set; } = null!;
        [Required(ErrorMessage = "Please Enter Valid Value")]
        [MaxLength(300 , ErrorMessage = "Store Location Must Be Great Than 9 Chars")]
        [MinLength(10 , ErrorMessage = "Store Location Must Be Great Than 9 Chars")]
        public string Location { get; set; } = null!; 
    }
}
