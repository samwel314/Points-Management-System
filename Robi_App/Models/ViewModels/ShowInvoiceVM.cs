using System.ComponentModel.DataAnnotations;

namespace Robi_App.Models.ViewModels
{
    public class ShowInvoiceVM
    {
        public int Id { get; set; }
        public string CustomerId { get; set; } = null!;
        public string storeName { get; set; } = null!;
        public string code { get; set; } = null!;
        [Range(minimum: 0, maximum: 2000)]
        public int Points { get; set; } 
        public string Date {  get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Phone { get; set; } = null!; 

    }
}
