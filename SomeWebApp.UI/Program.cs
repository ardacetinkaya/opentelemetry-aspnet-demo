using SomeWebApp.UI;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using OpenTelemetry.Extensions.Hosting.Implementation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<HeaderValidator>();


builder.Services.AddHttpClient("Main", httpClient =>
{
    var address = Environment.GetEnvironmentVariable("API_SERVICE_FQDN")?? "localhost";
    httpClient.BaseAddress = new Uri($"https://{address}");

}).AddHttpMessageHandler<HeaderValidator>();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services
    .AddOpenTelemetryTracing((builder) => builder
    .AddAspNetCoreInstrumentation()
    .AddJaegerExporter());

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
