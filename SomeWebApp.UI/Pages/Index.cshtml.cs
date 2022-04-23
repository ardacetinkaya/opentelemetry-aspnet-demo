using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
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
        public string Days { get; set; }
        
        private const string DEFAULT_DAY = "10";
        public IndexModel(ILogger<IndexModel> logger,IHttpClientFactory factory)
        {
            _logger = logger;
            _httpClientFactory = factory;
            _httpClient = _httpClientFactory.CreateClient("Main");

            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task OnGet(string days)
        {
            Days = days ?? DEFAULT_DAY;
            var query = new Dictionary<string, string>()
            {
                ["fordays"] = days ?? DEFAULT_DAY
            };

            var uri = QueryHelpers.AddQueryString("/WeatherForecast", query);

            //Some custom ID for this request as baggage
            var activityFeature = HttpContext.Features.Get<IHttpActivityFeature>();
            activityFeature?.Activity.AddBaggage("RequestId", Guid.NewGuid().ToString());
            
            var httpResponseMessage = await _httpClient.GetAsync(uri);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var contentString = await httpResponseMessage.Content.ReadAsStringAsync();
                Forecasts =  JsonSerializer.Deserialize<IEnumerable<WeatherForecast>>(contentString,_options);
            }
        }
    }
}