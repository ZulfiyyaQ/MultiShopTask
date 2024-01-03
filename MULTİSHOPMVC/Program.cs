using Microsoft.EntityFrameworkCore;
using MULTİSHOPMVC.DAL;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
var app = builder.Build();
app.UseRouting();
app.UseStaticFiles();
app.MapControllerRoute(
    "MultiShop",
    "{controller=home}/{action=index}/{id?}"
    );

app.Run();
