using Microsoft.EntityFrameworkCore;
using AdvancedFinalProject;
 
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });


var key = Encoding.ASCII.GetBytes("JWT_AdvancedFinalProject_Toekn_Key");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // True for production
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();


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

app.UseAuthentication(); 


app.UseRouting();
 
app.UseAuthorization();

 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Registration}/{action=SignUp}");

 app.MapControllers();  // This will map API routes (including /api/ProjectApi)

app.Run();
