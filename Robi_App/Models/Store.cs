using System.ComponentModel.DataAnnotations;

namespace Robi_App.Models
{
    public class Store
    {
        public int Id { get; set;  }
        [MinLength(5)]
        [MaxLength(70)]
        public string Title { get; set; } = null!;
        [MaxLength(300)]
        [MinLength(10)]
        public string Location { get; set; } = null!; 
    }


}
