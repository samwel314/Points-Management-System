using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Robi_App;
using Robi_App.Models;
using Robi_App.AuthorizationRequirement;
using Robi_App.AuthorizationRequirement.Handler;
using Robi_App.Data;
using Robi_App.Data.DBInitializer;
using Robi_App.Services;
using Robi_App.Services.Implementation;
using System;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IStoreService , StoreService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IDBInitializer, DBInitializer>();


// Authorization polices builder.Services.AddAuthorization(options =>

builder.Services.AddAuthorization(Opt =>
{
    Opt.AddPolicy("CanUpdateInvoice", apb =>
      apb.AddRequirements(new IsInvoiceOwnerRequirement()));
    Opt.AddPolicy(SD.Role_Admin, pb => pb.RequireClaim(SD.Role_Admin));
    Opt.AddPolicy(SD.Role_Client, pb => pb.RequireClaim(SD.Role_Client));
    Opt.AddPolicy(SD.Role_Employee, pb => pb.RequireClaim(SD.Role_Employee));

}); 

builder.Services.AddScoped<IAuthorizationHandler, InvoiceOwnerhandler>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
SeedDatabase(); 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<IDBInitializer>();
        db.Initialize();
    }
}