/// <summary>
/// Represents extension and helper methods for the tests.
/// </summary>
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;

namespace AspNetHeaderReplicator.Tests;

internal static class Helpers
{
    public static IEnumerable<string> GetEmptyEnumerable() => Enumerable.Empty<string>();
    public static string GetRandomValue() => new StringValues(Guid.NewGuid().ToString());

    public static IHeaderDictionary GetMergedHeaders(params IHeaderDictionary[] headers)
    {
        if(headers == null || headers.Length == 0)
            throw new ArgumentException("The headers cannot be null or empty.", nameof(headers));

        var result = new HeaderDictionary();
        foreach (var header in headers)
        {
            foreach (var (key, value) in header)
            {
                if(result.ContainsKey(key))
                    throw new ArgumentException($"The key {key} already exists in the result dictionary.", nameof(key));
                    
                result[key] = value;
            }
        }

        return result;
    }

    public static bool AreStringCollectionsEqualWithoutOrderAndCase(IEnumerable<string> expected, IEnumerable<string> actual)
    {
        if(expected == null || actual == null)
            throw new ArgumentNullException("The expected and actual collections cannot be null.");

        if(expected.Count() != actual.Count())
            throw new ArgumentException("The expected and actual collections must have the same count.");

        foreach (var item in expected)
        {
            if(!actual.Any(a => a.Equals(item, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"The item {item} does not exist in the actual collection.");
        }

        return true;
    }

    public static bool AreStringCollectionContainsWithoutOrderAndCase(IEnumerable<string> expected, IEnumerable<string> actual)
    {
        if(expected == null || actual == null)
            throw new ArgumentNullException("The expected and actual collections cannot be null.");

        foreach (var item in expected)
        {
            if(!actual.Any(a => a.Contains(item, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"The item {item} does not exist in the actual collection.");
        }

        return true;
    }

    public static bool AreHeaderDictionariesEqual(IHeaderDictionary expected, IHeaderDictionary actual)
    {
        if(expected == null || actual == null)
            throw new ArgumentNullException("The expected and actual headers cannot be null.");

        if(expected.Count != actual.Count)
            throw new ArgumentException("The expected and actual headers must have the same count.");

        foreach (var (key, value) in expected)
        {
            if(!actual.ContainsKey(key))
                throw new ArgumentException($"The key {key} does not exist in the actual headers.");

            var actualValue = actual[key];
            if(value.Count != actualValue.Count)
                throw new ArgumentException($"The key {key} has different values count.");

            for (int i = 0; i < value.Count; i++)
            {
                if(value[i] != actualValue[i])
                    throw new ArgumentException($"The key {key} has different values.");
            }
        }

        return true;
    }

    public static KeyValuePair<string, StringValues> CreateIgnoredHeader(IHeaderReplicatorConfiguration config)
    {
        var key = $"{GetIgnoredHeaderSentence(config)}-{Guid.NewGuid()}";
        return new KeyValuePair<string, StringValues>(key, new StringValues($"{nameof(CreateIgnoredHeader)}_{Guid.NewGuid()}"));
    }

    public static KeyValuePair<string, StringValues> CreateAllowedHeader(IHeaderReplicatorConfiguration config)
    {
        var key = $"{GetAllowedHeaderPrefix(config)}-{Guid.NewGuid()}";
        return new KeyValuePair<string, StringValues>(key, new StringValues($"{nameof(CreateAllowedHeader)}_{Guid.NewGuid()}"));
    }

    public static string GetIgnoredHeaderSentence(IHeaderReplicatorConfiguration config)
    {
        return $"{GetRandomItem(config.IgnoredHeaderSentences).TrimDash()}-Ignored";
    }

    public static string GetAllowedHeaderPrefix(IHeaderReplicatorConfiguration config)
    {
        return $"{GetRandomItem(config.AllowedHeaderPrefixes).TrimDash()}-Allowed";
    }


    public static T GetRandomItem<T>(IEnumerable<T> items)
    {
        if(items == null || items.Count() == 0)
            throw new ArgumentException("The items cannot be null or empty.", nameof(items));

        var random = new Random();
        var index = random.Next(0, items.Count() - 1);
        return items.ElementAt(index);
    }

    public static string TrimDash(this string value)
    {
        return value.Trim('-');
    }
}

internal static class Mocked
{
    // public static ILogger<T> GetLogger<T>()
    // {
    //     // Can not create proxy for type Microsoft.Extensions.Logging.ILogger
    //     var mockLogger = new Mock<ILogger<T>>();
    //     mockLogger
    //         .Setup(l => l.Log(
    //             It.IsAny<LogLevel>(),
    //             It.IsAny<EventId>(),
    //             It.IsAny<object>(),
    //             It.IsAny<Exception>(),
    //             It.IsAny<Func<object, Exception, string>>()
    //         ));

    //     return mockLogger.Object;
    // }


    public static IHeaderReplicatorConfiguration GetHeaderReplicatorConfigurationWithDefaults(bool allowAll)
    {
        var allowed = HeaderReplicatorConfigurationBuilder.__def_allowedHeaderPrefixes;
        var ignored = HeaderReplicatorConfigurationBuilder.__def_ignoredHeaderSentences;
        return GetHeaderReplicatorConfiguration(allowAll, allowed, ignored);
    }

    public static IHeaderReplicatorConfiguration GetHeaderReplicatorConfiguration(bool allowAll, IEnumerable<string> allowedPrefixes, IEnumerable<string> ignoredSentences)
    {
        var mockConfig = Mock.Of<IHeaderReplicatorConfiguration>((c) => c.AllowAll == allowAll && c.AllowedHeaderPrefixes == allowedPrefixes && c.IgnoredHeaderSentences == ignoredSentences);
        return mockConfig;
    }

    
}