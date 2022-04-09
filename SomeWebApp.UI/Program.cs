using SomeWebApp.UI;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using System.Reflection;
using OpenTelemetry.Exporter;
using SomeWebApp.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<HeaderValidator>();


builder.Services.AddHttpClient("Main", httpClient =>
{
    var address = Environment.GetEnvironmentVariable("API_SERVICE_FQDN") ?? "localhost";
    httpClient.BaseAddress = new Uri($"https://{address}");

}).AddHttpMessageHandler<HeaderValidator>();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddTracingSupport(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
