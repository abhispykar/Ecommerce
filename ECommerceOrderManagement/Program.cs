using ECommerceOrderManagement.Interfaces;
using Microsoft.EntityFrameworkCore;
using EOMS.DataAccess.Data;
using EOMS.DataAccess.Repository.IRepository;
using EOMS.DataAccess.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using EOMS.Utility;
using NLog.Extensions.Logging;
using ECommerceOrderManagement.GlobalExceptionHandler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddSignalR(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(15); // Ping every 15 seconds
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30); // Timeout after 30 seconds
});
builder.Services.AddRazorPages();
builder.Logging.ClearProviders(); // Clear default logging providers
builder.Logging.AddNLog("NLog.config"); // Add NLog using the configuration file
builder.Services.AddExceptionHandler<AppExceptionHandler>();
// Register ProductRepository as a scoped service instead of a singleton
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
builder.Services.AddScoped<IOrderHeaderRepository, OrderHeaderRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseExceptionHandler(_=> { });
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapAreaControllerRoute(
    name: "Customer",
    areaName: "Customer",
    pattern: "Customer/{controller=Home}/{action=Index}/{id?}"
);

app.MapAreaControllerRoute(
    name: "Admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}"
);

app.MapAreaControllerRoute(
    name: "default",
    areaName: "Customer",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}"
);
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<OrderStatusChangedHub>("/orderStatusHub");
});
app.Run();
