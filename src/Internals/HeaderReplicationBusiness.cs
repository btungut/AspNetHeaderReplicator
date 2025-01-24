/// <summary>
/// Represents the business logic for the header replicator which constructed by the <see cref="HeaderReplicatorMiddleware"/> and used to replicate the headers.
/// </summary>

using Microsoft.AspNetCore.Http;

namespace AspNetHeaderReplicator.Internals;

internal class HeaderReplicationBusiness
{
    public const string RedactedValue = $"REDACTED_By_{nameof(HeaderReplicationBusiness)}";

    private readonly IHeaderReplicatorConfiguration _config;

    internal HeaderReplicationBusiness(IHeaderReplicatorConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        if (_config.IgnoredHeaderSentences == null) throw new ArgumentNullException(nameof(config.IgnoredHeaderSentences));
        if (_config.AllowedHeaderPrefixes == null) throw new ArgumentNullException(nameof(config.AllowedHeaderPrefixes));
    }

    internal IHeaderDictionary GetReplicatedHeaders(IHeaderDictionary requestHeaders)
    {
        if (requestHeaders == null) throw new ArgumentNullException(nameof(requestHeaders));

        // Create a new dictionary to store the headers that will be added to the response.
        var replicatedHeaders = new HeaderDictionary();

        // If there are no headers, return an empty dictionary.
        if (requestHeaders.Count == 0)
            return replicatedHeaders;

        if (_config.AllowAll)
        {
            replicatedHeaders.AddOrReplaceRange(requestHeaders);
            return replicatedHeaders;
        }

        // Iterate over each header in the request headers.
        foreach (var header in requestHeaders)
        {
            if (header.Key == null || header.Value.Count == 0) continue;

            var key = header.Key;
            var value = header.Value;


            bool isIgnoredAlready = HeaderReplicationCache.Instance.IsIgnoredHeader(key);
            if (isIgnoredAlready)
            {
                replicatedHeaders[key] = RedactedValue;
                continue;
            }

            bool isIgnored = _config.IgnoredHeaderSentences.Any(ignoredSentence => key.IndexOf(ignoredSentence, StringComparison.OrdinalIgnoreCase) >= 0);
            if (isIgnored)
            {
                replicatedHeaders[key] = RedactedValue;
                HeaderReplicationCache.Instance.AddIgnoredHeader(key);
                continue;
            }

            bool isAllowedAlready = HeaderReplicationCache.Instance.IsAllowedHeader(key);
            if (isAllowedAlready)
            {
                replicatedHeaders[key] = value;
                continue;
            }

            var keyPrefix = GetHeaderKeyPrefix(key);
            var isKeyPrefixAllowed = _config.AllowedHeaderPrefixes.Contains(keyPrefix, StringComparer.OrdinalIgnoreCase);
            if (isKeyPrefixAllowed)
            {
                replicatedHeaders[key] = value;
                HeaderReplicationCache.Instance.AddAllowedHeader(key);
                continue;
            }
        }

        return replicatedHeaders;
    }

    private string GetHeaderKeyPrefix(string key)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("The key cannot be null or empty.", nameof(key));

        var indexOfDash = key.IndexOf('-');
        if (indexOfDash < 0) return key;

        return key.Substring(0, indexOfDash + 1);
    }
}