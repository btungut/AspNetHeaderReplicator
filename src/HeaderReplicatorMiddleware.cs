
using DotNetHeaderReplicator;

namespace Microsoft.AspNetCore.Http;

public class HeaderReplicatorMiddleware : IMiddleware
{
    private readonly HeaderReplicatorConfigurationBuilder.HeaderReplicatorConfiguration _config;

    public HeaderReplicatorMiddleware(HeaderReplicatorConfigurationBuilder.HeaderReplicatorConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (next == null) throw new ArgumentNullException(nameof(next));


        var business = new HeaderReplicationBusiness(_config.AllowAll, _config.AllowedHeaderPrefixes, _config.IgnoredHeaderSentences);
        var toBeModifiedHeaders = business.GetReplicatedHeaders(context.Request.Headers);

        if (toBeModifiedHeaders.Count != 0)
        {
            context.Response.OnStarting(() =>
            {
                foreach (var header in toBeModifiedHeaders)
                {
                    context.Response.Headers[header.Key] = header.Value;
                }

                return Task.CompletedTask;
            });
        }

        await next(context);
    }
}