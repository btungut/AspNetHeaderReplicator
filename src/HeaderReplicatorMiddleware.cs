
using DotNetHeaderReplicator;
using DotNetHeaderReplicator.Internals;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Http;

public class HeaderReplicatorMiddleware : IMiddleware
{
    private readonly IHeaderReplicatorConfiguration _config;
    private readonly LoggerFactory _loggerFactory;

    public HeaderReplicatorMiddleware(IHeaderReplicatorConfiguration config, LoggerFactory loggerFactory)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (next == null) throw new ArgumentNullException(nameof(next));

        var business = new HeaderReplicationBusiness(_config, _loggerFactory.CreateLogger<HeaderReplicationBusiness>());

        // Add the replicated headers from request to the response when the response is starting.
        context.Response.OnStarting(() =>
        {
            var replicatedHeaders = business.GetReplicatedHeaders(context.Request.Headers);
            context.Response.Headers.AddOrReplaceRange(replicatedHeaders);
            return Task.CompletedTask;
        });

        await next(context);
    }
}