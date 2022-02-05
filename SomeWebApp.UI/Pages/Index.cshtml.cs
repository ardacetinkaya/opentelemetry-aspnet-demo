using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace SomeWebApp.UI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public IEnumerable<WeatherForecast>? Forecasts { get; private set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            //_httpClientFactory = factory;
            // _httpClient = _httpClientFactory.CreateClient("Main");

            // _options = new JsonSerializerOptions
            // {
            //     PropertyNameCaseInsensitive = true
            // };
        }

        public async Task OnGet()
        {
            // var httpResponseMessage = await _httpClient.GetAsync("/WeatherForecast");
            // if (httpResponseMessage.IsSuccessStatusCode)
            // {
            //     var contentString = await httpResponseMessage.Content.ReadAsStringAsync();
            //     Forecasts =  JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(contentString,_options);
            // }
        }
    }
}