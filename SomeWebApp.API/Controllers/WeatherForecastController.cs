using Microsoft.AspNetCore.Mvc;

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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast/{fordays}")]
        public IEnumerable<WeatherForecast> Get(int fordays=5)
        {
            _logger.LogInformation("Weather Forecast for {days} days is requested.", fordays);
            var tempratures =  Enumerable.Range(1, fordays).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-55, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            _logger.LogInformation("Weather Forecast for {days} days is calculated. Higher temprature is {temp} on {day}"
            , fordays
            , tempratures.Max(x => x.TemperatureC)
            , tempratures.First(x => x.TemperatureC == tempratures.Max(y => y.TemperatureC)).Date.ToString("dd/MM/yyyy"));

            return tempratures;
        }
    }
}
