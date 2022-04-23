using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using SomeWebApp.API.Data;

namespace SomeWebApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ForecastContext _context;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, ForecastContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet(Name = "GetWeatherForecast/{fordays}")]
        public async Task<IEnumerable<WeatherForecast>> Get(int fordays=5)
        {
            _logger.LogInformation(_context.Forecasts.Count().ToString()+" records in database");
            _logger.LogInformation("Weather Forecast for {days} days is requested.", fordays);

            //Get HTTP Activity feature
            var activityFeature = HttpContext.Features.Get<IHttpActivityFeature>();
            //Get the custom added RequestId value from baggage
            //Add it as event data
            var requestId= activityFeature?.Activity.GetBaggageItem("RequestId");
            var @event = new ActivityEvent($"Custom added RequestId: {requestId}"??"Unknown");
            Activity.Current?.AddEvent(@event);
            
            var tempratures =  Enumerable.Range(1, fordays).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-55, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            _logger.LogInformation("Weather Forecast for {days} days is calculated. Higher temprature is {temp} on {day}."
            , fordays
            , tempratures.Max(x => x.TemperatureC)
            , tempratures.First(x => x.TemperatureC == tempratures.Max(y => y.TemperatureC)).Date.ToString("dd/MM/yyyy"));

            _context.Add(new Forecast
            {
                TraceId = Activity.Current?.TraceId.ToString(),
                Date = DateOnly.FromDateTime(DateTime.Now),
                MaxTemperatureC = tempratures.Max(x => x.TemperatureC),
                MinTemperatureC = tempratures.Min(x => x.TemperatureC),
                Summary = "Weather Forecast for " + fordays + " days is calculated. Higher temprature is " + tempratures.Max(x => x.TemperatureC) + " on " + tempratures.First(x => x.TemperatureC == tempratures.Max(y => y.TemperatureC)).Date.ToString("dd/MM/yyyy")
            });

            await _context.SaveChangesAsync();

            return tempratures;
        }
    }
}
