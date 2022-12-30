using asplib.Services;
using BlazorApp1.Data;
using BlazorApp1.Models;
using iselenium;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddPersistent<TestRunnerFsm>();    // TestButton SMC FSM
builder.Services.AddPersistent<CounterModel>();     // The persisted model of the app

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseMiddleware<ISeleniumMiddleware>();   // iselenium for asserting the HTTP status code
app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();