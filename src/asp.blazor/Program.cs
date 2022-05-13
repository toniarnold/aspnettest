using asp.blazor.CalculatorSmc;
using asplib.Model.Db;
using asplib.Services;
using iselenium;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
ASP_DBEntities.ConnectionString = builder.Configuration["ASP_DBEntities"];  // legacy .NET Framework pattern
builder.Services.AddHttpContextAccessor();      // for TestButton
builder.Services.AddPersistent<Calculator>();   // asplib
builder.Services.AddHttpContextAccessor();      // for TestButton
builder.Services.AddLogging();

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