namespace Robi_App.Models.ViewModels
{
    public class ShowStoreInvoicesVM {
    
        public int Id { get; set; }
        public string StoreName { get; set; } = null!; 
        public int TotalInvoices { get; set; }  
        public int TotalToDayInvoices { get; set; }
        public int TotalToMonthInvoices { get;set; }
        public IEnumerable<ShowInvoiceVM> Invoices { get; set; } = null!;     

    }

}
