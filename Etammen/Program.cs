using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Repositories;
using DataAccessLayerEF.Context;
using DataAccessLayerEF.Models;
using Etammen.Mapping.ClinicForAdmin;
using Etammen.Mapping.DoctorForAdmin;
using Etammen.Mapping.PatientForAdmin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    //options.Filters.Add(new AuthorizeFilter());
});

builder.Services.AddDbContext<EtammenDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
        ,b=>b.MigrationsAssembly(typeof(EtammenDbContext).Assembly.FullName));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.SignIn.RequireConfirmedEmail = true;
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.MaxFailedAccessAttempts = 5;

}).AddEntityFrameworkStores<EtammenDbContext>()
  .AddDefaultTokenProviders();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Admin Services
builder.Services.AddScoped<DoctorsAdminMapper>();
builder.Services.AddScoped<ClinicAdminMapper>();
builder.Services.AddScoped<PatientForAdminMapper>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Home}/{id?}");

app.Run();
