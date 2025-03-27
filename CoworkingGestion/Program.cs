using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using CoworkingGestion.Models;


var builder = WebApplication.CreateBuilder(args);
AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true);


// Configurar la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSQL")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});






// Configurar autenticación con cookies (SIN Identity)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Redirigir a login si no está autenticado
        options.AccessDeniedPath = "/Home/AccessDenied"; // Redirigir si el usuario no tiene permisos
    });

builder.Services.AddControllersWithViews();
builder.Services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();



// Configurar las rutas del proyecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.UseSession();
app.Run();
