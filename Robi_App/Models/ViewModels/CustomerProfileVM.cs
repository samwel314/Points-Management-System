namespace Robi_App.Models.ViewModels
{
    public class CustomerProfileVM
    {
        public int TotalPoints { get; set; }
        public string CustomerId { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!; 
        public string CustomerName { get; set; } = null!;
        public  IEnumerable<ShowInvoiceVM> Invoices { get; set; } = null!;
    }
}
