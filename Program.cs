using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ProyectoRelampago;
using ProyectoRelampago.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<RedirectToLogin>();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddPageApplicationModelConvention("/Login", model =>
    {
        model.Filters.Add(new RedirectToLogin());
    });
   
});
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Login";
        options.Cookie.Name = "Cookies";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });
builder.Services.AddDbContext<Context>(options =>
 options.UseSqlServer(builder.Configuration.GetConnectionString("Host")));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapGet("/", context =>
{
    context.Response.Redirect("/Login");
    return Task.CompletedTask;
});
app.Run();
