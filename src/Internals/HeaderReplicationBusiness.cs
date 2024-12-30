using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DotNetHeaderReplicator.Internals;

public class HeaderReplicationBusiness
{
    public const string RedactedValue = $"REDACTED_By_{nameof(HeaderReplicationBusiness)}";

    private readonly IHeaderReplicatorConfiguration _config;
    private readonly ILogger<HeaderReplicationBusiness> _logger;

    internal HeaderReplicationBusiness(IHeaderReplicatorConfiguration config, ILogger<HeaderReplicationBusiness> logger)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        if(_config.IgnoredHeaderSentences == null) throw new ArgumentNullException(nameof(config.IgnoredHeaderSentences));
        if(_config.AllowedHeaderPrefixes == null) throw new ArgumentNullException(nameof(config.AllowedHeaderPrefixes));

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogDebug("HeaderReplicationBusiness instance created with config {@config}", config);
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
            _logger.LogDebug("AllowAll is true. All headers will be replicated {@logCtx}", new
            {
                sourceHeaders = requestHeaders,
                replicatedHeaders = replicatedHeaders,
                config = _config
            });
            return replicatedHeaders;
        }

        _logger.LogDebug("AllowAll is false. Only the headers that match the allowed prefixes and also do not contain the ignored sentences will be replicated {@config}", _config);

        // Iterate over each header in the request headers.
        foreach (var header in requestHeaders)
        {
            if (header.Key == null || header.Value.Count == 0) continue;

            var key = header.Key;
            var value = header.Value;

            bool isIgnored = _config.IgnoredHeaderSentences.Any(sentence => key.Contains(sentence, StringComparison.OrdinalIgnoreCase));
            if (isIgnored)
            {
                value = RedactedValue;
                continue;
            }

            var keyPrefix = GetHeaderKeyPrefix(key);
            var isKeyPrefixAllowed = _config.AllowedHeaderPrefixes.Contains(keyPrefix, StringComparer.OrdinalIgnoreCase);

            if (isKeyPrefixAllowed)
            {
                replicatedHeaders[key] = value;
                continue;
            }

            throw new InvalidOperationException($"The request header key '{key}' with '{value}' is not determined to be replicated. " +
                $"The key prefix '{keyPrefix}' is not allowed. " +
                $"The allowed prefixes are '{string.Join(", ", _config.AllowedHeaderPrefixes)}'. " +
                $"The ignored sentences are '{string.Join(", ", _config.IgnoredHeaderSentences)}'.");
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

