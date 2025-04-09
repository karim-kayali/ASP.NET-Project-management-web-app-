
using Microsoft.EntityFrameworkCore;
using AdvancedFinalProject;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddControllersWithViews();



builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseMySql(builder.Configuration.GetConnectionString("connection"),
        new MySqlServerVersion(new Version(8, 0, 25))));


builder.Services.AddSession();
builder.Services.AddHttpContextAccessor(); // Optional but good to have


var app = builder.Build();


app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

 

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Registration}/{action=SignUp}");


app.Run();
