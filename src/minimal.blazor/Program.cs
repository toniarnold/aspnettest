using asplib.Model.Db;
using asplib.Services;
using iselenium;
using minimal.blazor.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
ASP_DBEntities.ConnectionString = builder.Configuration["ASP_DBEntities"];  // legacy .NET Framework pattern static reference
builder.Services.AddSingleton<IServiceProvider>(sp => sp);  // asplib.PersistentComponentBase<T>
builder.Services.AddHttpContextAccessor();                  // for TestButton
builder.Services.AddPersistent<TestRunnerFsm>();            // TestButton SMC FSM
builder.Services.AddPersistent<Main>();                     // asplib

var app = builder.Build();

// Configure the HTTP request pipeline.
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