
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using System;
using System.Reflection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

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
                .AddHttpClientInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.Enrich = HttpActivityEnrichment;
                })
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.Enrich = AspNetCoreActivityEnrichment;
                });

            switch (tracingExporter)
            {
                case "jaeger":
                    options.AddJaegerExporter();
                    break;
                default:
                    options.AddConsoleExporter();
                    break;
            }
        });

        return services;
    }

    private static void HttpActivityEnrichment(Activity activity, string eventName, object rawObject)
    {
        if (eventName.Equals("OnStartActivity") && rawObject is HttpRequestMessage httpRequest)
        {
            var request = "EMPTY!";
            if (httpRequest.Content != null)
            {
                request = httpRequest.Content.ReadAsStringAsync().Result;

            }
            activity.SetTag("http.request_content", request);

        }

        if (eventName.Equals("OnStopActivity") && rawObject is HttpResponseMessage httpResponse)
        {
            var response = "EMPTY!";
            if (httpResponse.Content != null)
            {
                response = httpResponse.Content.ReadAsStringAsync().Result;
            }

            activity.SetTag("http.response_content", response);

        }

        if (eventName.Equals("OnException") && rawObject is Exception exception)
        {
            activity.SetTag("http.exception", exception.Message);
        }

        SetTraceId(activity);
    }

    private static void AspNetCoreActivityEnrichment(Activity activity, string eventName, object rawObject)
    {
        if((activity.GetTagItem("http.target") ?? "/") == "/")
               activity.DisplayName="Home";
        else
            activity.DisplayName = activity.GetTagItem("http.target")?.ToString();

        SetTraceId(activity);
    }

    private static void SetTraceId(Activity activity)
    {
        activity.AddTag("trc.TraceId", activity.TraceId.ToString());
        activity.AddTag("trc.TraceSpanId", activity.SpanId.ToString());
        activity.AddTag("trc.TraceParentSpanId", activity.Parent?.SpanId.ToString());
    }
}
