namespace Robi_App.Models.ViewModels
{
    public class ShowInvoiceVM
    {
        public int Id { get; set; }
        public string storeName { get; set; } = null!;
        public string code { get; set; } = null!;
        public int Points { get; set; } 
        public string Date {  get; set; } = null!;

    }
}
