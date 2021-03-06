using SomeWebApp.UI;
using SomeWebApp.Telemetry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<HeaderValidator>();


builder.Services.AddHttpClient("Main", httpClient =>
{
    var uri = builder.Configuration.GetServiceUri(name:"api",binding:"http");
    httpClient.BaseAddress = uri;

}).AddHttpMessageHandler<HeaderValidator>();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddTracingSupport(builder.Configuration);
builder.Logging.AddLoggingSupport(builder.Configuration);

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
