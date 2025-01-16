/// <summary>
/// Represents the middleware that replicates headers from the request to the response.
/// </summary>
﻿

using DotNetHeaderReplicator.Internals;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DotNetHeaderReplicator;

public class HeaderReplicatorMiddleware : IMiddleware
{
    private readonly IHeaderReplicatorConfiguration _config;
    private readonly ILogger _logger;

    public HeaderReplicatorMiddleware(IHeaderReplicatorConfiguration config, ILogger<HeaderReplicatorMiddleware> logger = null)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (next == null) throw new ArgumentNullException(nameof(next));

        var business = new HeaderReplicationBusiness(_config, _logger);

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