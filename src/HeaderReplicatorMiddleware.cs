
namespace Microsoft.AspNetCore.Http;

public class HeaderReplicatorMiddleware : IMiddleware
{
    internal const string RedactedValue = $"REDACTED_by_{nameof(HeaderReplicatorMiddleware)}";
    private readonly HeaderReplicatorConfigurationBuilder.HeaderReplicatorConfiguration _config;

    public HeaderReplicatorMiddleware(HeaderReplicatorConfigurationBuilder.HeaderReplicatorConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (next == null)
            throw new ArgumentNullException(nameof(next));


        IHeaderDictionary toBeAddedHeaders = new HeaderDictionary();
        var requestHeaders = context.Request.Headers;

        foreach (var header in requestHeaders)
        {
            if (header.Key == null || header.Value.Count == 0)
                continue;

            var key = header.Key;
            var keyPrefix = GetHeaderKeyPrefix(key);
            var value = header.Value;

            if (_config.AllowAll)
            {
                toBeAddedHeaders[key] = value;
                continue;
            }

            if (_config.AllowedHeaderPrefixes.Contains(keyPrefix, StringComparer.OrdinalIgnoreCase))
            {
                if (_config.IgnoredHeaderSentences.Any(sentence => key.Contains(sentence, StringComparison.OrdinalIgnoreCase)))
                {
                    value = RedactedValue;
                }

                toBeAddedHeaders[key] = value;
            }
        }

        context.Response.OnStarting(() =>
        {
            foreach (var header in toBeAddedHeaders)
            {
                context.Response.Headers[header.Key] = header.Value;
            }

            return Task.CompletedTask;
        });

        await next(context);
    }

    private string GetHeaderKeyPrefix(string key)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("The key cannot be null or empty.", nameof(key));

        var indexOfDash = key.IndexOf('-');
        if (indexOfDash < 0) return key;

        return key.Substring(0, indexOfDash + 1);
    }
}





