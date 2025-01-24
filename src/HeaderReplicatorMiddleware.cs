/// <summary>
/// Represents the middleware that replicates headers from the request to the response.
/// </summary>


using AspNetHeaderReplicator.Internals;
using Microsoft.AspNetCore.Http;

namespace AspNetHeaderReplicator;

public class HeaderReplicatorMiddleware : IMiddleware
{
    private readonly IHeaderReplicatorConfiguration _config;

    public HeaderReplicatorMiddleware(IHeaderReplicatorConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (next == null) throw new ArgumentNullException(nameof(next));

        var business = new HeaderReplicationBusiness(_config);

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