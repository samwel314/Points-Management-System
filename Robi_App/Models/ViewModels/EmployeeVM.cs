namespace Robi_App.Models.ViewModels
{
    public class EmployeeVM
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string StoreName { get; set; }   
        public bool IsLocked { get; set; }      
        public string PhoneNumber { get; set; } = null!;
        public string PassWord { get; set; } = null!;
    }
}
