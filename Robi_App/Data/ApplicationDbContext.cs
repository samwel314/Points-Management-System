using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Robi_App.Models;

namespace Robi_App.Data
{
    public class ApplicationDbContext : IdentityDbContext 
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Invoice> Invoices { get; set; }    
        public DbSet<Store> Stores { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // this is for identity tables
            base.OnModelCreating(builder);
            // Additional model configurations can go here

            builder.Entity<Store>().HasData(new List <Store>() 
            { 
                new Store
                {
                    Id = 1,
                    Title = "Main Street Store",
                    Location = "123 Main St, Springfield"
                }
                , 
                new Store
                {
                    Id = 2,
                    Title = "Downtown Store",
                    Location = "456 Elm St, Springfield"
                },
                new Store
                {
                   Id = 3,  
                   Title = "Uptown Store",
                   Location = "789 Oak St, Springfield"
                }
            }); 
        
        
        }

    }
}
