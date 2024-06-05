using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shops.Areas.Identity.Data;
using Shops.Models;

var builder = WebApplication.CreateBuilder(args);

// Mengambil connection string dari konfigurasi
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' tidak ditemukan.");

// Menambahkan layanan untuk DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<ProductsDbContext>(options => options.UseSqlServer(connectionString));

// Konfigurasi layanan identitas
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

// Menambahkan UserManager sebagai layanan yang disediakan
builder.Services.AddScoped<UserManager<ApplicationUser>>();

// Menambahkan layanan untuk HttpClient
builder.Services.AddHttpClient<EmployeeService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7268/");
});

// Menambahkan layanan untuk Controller dan View
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Konfigurasi pipeline HTTP request
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Menetapkan rute default untuk Controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
