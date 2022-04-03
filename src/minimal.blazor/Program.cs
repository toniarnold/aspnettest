using asplib.Model.Db;
using asplib.Services;
using iselenium;
using minimal.blazor.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
ASP_DBEntities.ConnectionString = builder.Configuration["ASP_DBEntities"];  // legacy .NET Framework pattern
builder.Services.AddHttpContextAccessor();
builder.Services.AddPersistent<Main>();     // asplib

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseMiddleware<ISeleniumMiddleware>();   // iselenium
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();