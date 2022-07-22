using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Flour.Metrics.Prometheus;

internal class PrometheusMiddleware : IMiddleware
{
    private readonly ISet<string> _allowedHosts;
    private readonly string _apiKey;
    private readonly string _endpoint;

    public PrometheusMiddleware(
        IOptions<PrometheusOptions> options)
    {
        var settings = options.Value
                       ?? throw new ArgumentNullException(nameof(options.Value), "Prometheus settings are not set");

        _allowedHosts = new HashSet<string>(settings.AllowedHosts ?? Array.Empty<string>());
        _apiKey = settings.ApiKey;
        _endpoint = string.IsNullOrWhiteSpace(settings.Endpoint)
            ? "/metrics"
            : $"/{settings.Endpoint.Trim('/')}";
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var request = context.Request;
        if (context.Request.Path != _endpoint) return next(context);

        if (string.IsNullOrWhiteSpace(_apiKey)) return next(context);

        if (request.Query.TryGetValue("apiKey", out var apiKey) && apiKey == _apiKey) return next(context);

        var host = context.Request.Host.Host;
        if (_allowedHosts.Contains(host)) return next(context);

        if (!request.Headers.TryGetValue("x-forwarded-for", out var forwardedFor))
        {
            context.Response.StatusCode = 404;
            return Task.CompletedTask;
        }

        if (_allowedHosts.Contains(forwardedFor)) return next(context);

        context.Response.StatusCode = 404;
        return Task.CompletedTask;
    }
}