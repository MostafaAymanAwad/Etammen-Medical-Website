using AutoMapper;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Repositories;
using DataAccessLayerEF.Context;
using DataAccessLayerEF.Models;
using Etammen.MappingProfile;
using Etammen.Mapping.ClinicForAdmin;
using Etammen.Mapping.DoctorForAdmin;
using Etammen.Mapping.PatientForAdmin;
using Etammen.Mapping_Profiles;
using Etammen.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Etammen.Mapping;
using Etammen.Services.ServicesConfigurations;
using Etammen.Services.Email;
using Microsoft.AspNetCore.Authentication.Cookies;
using BusinessLogicLayer.Services.ServicesConfigurations;
using Etammen.Helpers;
using Serilog;
using Serilog.Events;
using Etammen.GlobalExceptionHandlingMiddleware;
using System.Net;
using BusinessLogicLayer.Services.SMS;


var builder = WebApplication.CreateBuilder(args);

//configuring SeriLog
var loggerConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(loggerConfig).CreateLogger();


builder.Services.AddSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(jsonOptions =>
{
    jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
});



builder.Services.AddDbContext<EtammenDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
        , b => b.MigrationsAssembly(typeof(EtammenDbContext).Assembly.FullName));
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.SignIn.RequireConfirmedEmail = true;
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";

}).AddEntityFrameworkStores<EtammenDbContext>()
  .AddDefaultTokenProviders()
  .AddTokenProvider<EmailConfirmationTokenProvider<ApplicationUser>>("emailconfirmation")
  .AddRoles<IdentityRole>();


builder.Services.Configure<EmailConfirmationTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromDays(7);
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(2);
});

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAutoMapper(typeof(PatientProfile));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Expiration = TimeSpan.FromDays(14);
    options.LoginPath = "/Account/Login";
});

builder.Services.AddAuthentication().AddFacebook("facebook", options =>
{
    var facebookAuth = builder.Configuration.GetSection("Authentication:Facebook");
    options.ClientId = facebookAuth["AppId"];
    options.ClientSecret = facebookAuth["AppSecret"];
    options.SignInScheme = IdentityConstants.ExternalScheme;
});

builder.Services.AddAuthentication().AddGoogle("google", options =>
{
    var googleAuth = builder.Configuration.GetSection("Authentication:Google");
    options.ClientId = googleAuth["ClientId"];
    options.ClientSecret = googleAuth["ClientSecret"];
    options.SignInScheme = IdentityConstants.ExternalScheme;
});



builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));
builder.Services.Configure<SmsConfiguration>(builder.Configuration.GetSection("Twillio"));
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeConfiguration"));



builder.Services.AddTransient<IEmailService,EmailService>();
builder.Services.AddTransient<ISmsService, SmsService>();



builder.Services.AddTransient<DoctorRegisterationHelper>();
builder.Services.AddTransient<AccountMapper>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddAutoMapper(M => M.AddProfile(new DoctorProfile()));

builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));



builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IDoctorDetailsRepository, DoctorDetailsRepository>();
 

//Admin Services
builder.Services.AddScoped<DoctorsAdminMapper>();
builder.Services.AddScoped<ClinicAdminMapper>();
builder.Services.AddScoped<PatientForAdminMapper>();
builder.Services.AddScoped<DoctorReviewMapping>();





var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
}
else
{
    app.UseDeveloperExceptionPage();
}


app.UseStaticFiles();

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        int statusCode = httpContext.Response.StatusCode;
        if (statusCode >= 200 && statusCode <= 399)
        {
            return LogEventLevel.Information;
        }
        else if (statusCode >= 400)
        {
            return LogEventLevel.Warning;
        }
        else
        {
            return LogEventLevel.Information;
        }
    };
});

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/{id?}");

app.UseStatusCodePagesWithRedirects("/StatusCodeError/{0}");

app.Run();
