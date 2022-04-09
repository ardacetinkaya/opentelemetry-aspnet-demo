namespace SomeWebApp.API.Data;

internal class Forecast
{
    public int Id { get; set; }
    public string TraceId { get; set; }
    public DateOnly Date { get; set; }
    public int MaxTemperatureC { get; set; }
    public int MinTemperatureC { get; set; }
    public string Summary { get; set; }
}