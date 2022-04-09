
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using System;
using System.Reflection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Microsoft.Extensions.Configuration;

namespace SomeWebApp.Common;
public static class TracingSupportExtensions
{
    public static IServiceCollection AddTracingSupport(this IServiceCollection services, IConfiguration configuration)
    {
        var tracingExporter = configuration.GetValue<string>("TracingExporter").ToLowerInvariant();
        var appName = configuration.GetValue<string>("AppName").ToLowerInvariant();
        var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(appName, serviceVersion: assemblyVersion, serviceInstanceId: Environment.MachineName);


        services.AddOpenTelemetryTracing((options) =>
        {
            options.SetResourceBuilder(resourceBuilder)
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation();

            switch (tracingExporter)
            {
                case "jaeger":
                    options.AddJaegerExporter();
                    services.Configure<JaegerExporterOptions>(configuration.GetSection("Jaeger"));
                    // Customize the HttpClient that will be used when JaegerExporter is configured for HTTP transport.
                    // builder.Services.AddHttpClient("JaegerExporter", configureClient: (client) => client.DefaultRequestHeaders.Add("X-MyCustomHeader", "value"));
                    break;
                default:
                    options.AddConsoleExporter();
                    break;
            }
        });

        return services;
    }

}
