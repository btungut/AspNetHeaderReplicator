using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DotNetHeaderReplicator;


//TODO: Config class ctor is being called new HashSet twice...! Refactor this class to avoid this.
public class HeaderReplicationBusiness
{
    public const string RedactedValue = $"REDACTED_By_{nameof(HeaderReplicationBusiness)}";

    public bool AllowAll { get; private set; }
    public HashSet<string> AllowedHeaderPrefixes { get; private set; }
    public HashSet<string> IgnoredHeaderSentences { get; private set; }

    public HeaderReplicationBusiness(bool allowAll, IEnumerable<string> allowedPrefixes, IEnumerable<string> ignoredSentences)
    {
        AllowAll = allowAll;

        if(!AllowAll && (allowedPrefixes == null || allowedPrefixes.Count() == 0))
            throw new ArgumentNullException("The allowedPrefixes cannot be null or empty when AllowAll is set to false.");

        if(!AllowAll && ignoredSentences == null)
            throw new ArgumentNullException("The ignoredSentences cannot be null when AllowAll is set to false.");

        AllowedHeaderPrefixes = AllowAll ? new HashSet<string>() : new HashSet<string>(allowedPrefixes, StringComparer.OrdinalIgnoreCase);
        IgnoredHeaderSentences = AllowAll ? new HashSet<string>() : new HashSet<string>(ignoredSentences, StringComparer.OrdinalIgnoreCase);
    }

    public IHeaderDictionary GetReplicatedHeaders(IHeaderDictionary requestHeaders)
    {
        if (requestHeaders == null) throw new ArgumentNullException(nameof(requestHeaders));

        var toBeAddedHeaders = new HeaderDictionary();

        foreach (var header in requestHeaders)
        {
            if (header.Key == null || header.Value.Count == 0) continue;

            var key = header.Key;
            var value = header.Value;

            if (AllowAll)
            {
                toBeAddedHeaders[key] = value;
                continue;
            }

            var keyPrefix = GetHeaderKeyPrefix(key);

            if (AllowedHeaderPrefixes.Contains(keyPrefix, StringComparer.OrdinalIgnoreCase))
            {
                if (IgnoredHeaderSentences.Any(sentence => key.Contains(sentence, StringComparison.OrdinalIgnoreCase)))
                {
                    value = RedactedValue;
                }

                toBeAddedHeaders[key] = value;
            }
        }

        return toBeAddedHeaders;
    }

    private string GetHeaderKeyPrefix(string key)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("The key cannot be null or empty.", nameof(key));

        var indexOfDash = key.IndexOf('-');
        if (indexOfDash < 0) return key;

        return key.Substring(0, indexOfDash + 1);
    }

}

