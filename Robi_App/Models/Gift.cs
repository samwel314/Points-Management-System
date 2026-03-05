namespace Robi_App.Models
{
    public class Gift
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!; 
        public string ImagePath { get; set; } = null!;      
        public int Points { get; set; }     
    }
}
