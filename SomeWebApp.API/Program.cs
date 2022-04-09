using Microsoft.EntityFrameworkCore;
using SomeWebApp.API.Data;
using SomeWebApp.Telemetry;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ForecastContext>(options =>{
    options.UseNpgsql(builder.Configuration.GetConnectionString("ForecastContext")
    ,options => options.SetPostgresVersion(new Version(9, 6)));
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTracingSupport(builder.Configuration);
builder.Logging.AddLoggingSupport(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ForecastContext>();
    context.Database.EnsureCreated();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
