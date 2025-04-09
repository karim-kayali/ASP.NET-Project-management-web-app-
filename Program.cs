using Microsoft.EntityFrameworkCore;
using AdvancedFinalProject;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddControllersWithViews();  // For MVC views

// Register your DbContext with MySQL
builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseMySql(builder.Configuration.GetConnectionString("connection"),
        new MySqlServerVersion(new Version(8, 0, 25))));

builder.Services.AddSession();  // For managing session state
builder.Services.AddHttpContextAccessor(); // Optional but good to have

// Add HttpClient to the services for API calls
builder.Services.AddHttpClient();

// Add controllers for API and MVC
builder.Services.AddControllers(); // For API
builder.Services.AddControllersWithViews(); // For MVC

var app = builder.Build();

// Use Session for the app
app.UseSession();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();   


app.UseRouting();

// For Authorization (if needed)
app.UseAuthorization();

// Map MVC routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Registration}/{action=SignUp}");

// Map API routes
app.MapControllers();  // This will map API routes (including /api/ProjectApi)

app.Run();
