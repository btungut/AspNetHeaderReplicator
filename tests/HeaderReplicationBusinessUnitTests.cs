/// <summary>
/// Represents the business unit tests for the <see cref="HeaderReplicationBusiness"/> class.
/// </summary>

using AspNetHeaderReplicator.Internals;
using Microsoft.AspNetCore.Http;

namespace AspNetHeaderReplicator.Tests;

public class HeaderReplicationBusinessUnitTests
{
    [Fact]
    public void Ctor_ShouldThrowException_WhenConfigIsNull()
    {
        // Arrange
        var mockLogger = Mocked.GetLogger<HeaderReplicationBusiness>();

        // Act
        Action act = () => new HeaderReplicationBusiness(null, mockLogger);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Ctor_ShouldThrowException_WhenConfigIgnoredHeaderSentencesIsNull()
    {
        // Arrange
        var mockLogger = Mocked.GetLogger<HeaderReplicationBusiness>();
        var mockConfig = Mocked.GetHeaderReplicatorConfiguration(false, Helpers.GetEmptyEnumerable(), null);

        // Act
        Action act = () => new HeaderReplicationBusiness(mockConfig, mockLogger);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Ctor_ShouldThrowException_WhenConfigAllowedHeaderPrefixesIsNull()
    {
        // Arrange
        var mockLogger = Mocked.GetLogger<HeaderReplicationBusiness>();
        var mockConfig = Mocked.GetHeaderReplicatorConfiguration(false, null, Helpers.GetEmptyEnumerable());

        // Act
        Action act = () => new HeaderReplicationBusiness(mockConfig, mockLogger);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void GetReplicatedHeaders_ShouldThrowException_WhenRequestHeadersIsNull()
    {
        // Arrange
        var mockLogger = Mocked.GetLogger<HeaderReplicationBusiness>();
        var mockConfig = Mocked.GetHeaderReplicatorConfiguration(false, Helpers.GetEmptyEnumerable(), Helpers.GetEmptyEnumerable());
        var business = new HeaderReplicationBusiness(mockConfig, mockLogger);

        // Act
        Action act = () => business.GetReplicatedHeaders(null);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void GetReplicatedHeaders_ShouldReturnEmptyDictionary_WhenRequestHeadersIsEmpty()
    {
        // Arrange
        var mockLogger = Mocked.GetLogger<HeaderReplicationBusiness>();
        var mockConfig = Mocked.GetHeaderReplicatorConfiguration(false, Helpers.GetEmptyEnumerable(), Helpers.GetEmptyEnumerable());
        var business = new HeaderReplicationBusiness(mockConfig, mockLogger);
        var requestHeaders = new HeaderDictionary();

        // Act
        var result = business.GetReplicatedHeaders(requestHeaders);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetReplicatedHeaders_ShouldReturnAllHeaders_WhenAllowAllIsTrue()
    {
        // Arrange
        var mockLogger = Mocked.GetLogger<HeaderReplicationBusiness>();
        var mockConfig = Mocked.GetHeaderReplicatorConfigurationWithDefaults(true);
        var business = new HeaderReplicationBusiness(mockConfig, mockLogger);

        // Arrange - Request Headers
        var __requestHeaders_ignored = new HeaderDictionary
        {
            Helpers.CreateIgnoredHeader(mockConfig),
            Helpers.CreateIgnoredHeader(mockConfig),
            Helpers.CreateIgnoredHeader(mockConfig),
        };
        var __requestHeaders_allowed = new HeaderDictionary
        {
            Helpers.CreateAllowedHeader(mockConfig),
            Helpers.CreateAllowedHeader(mockConfig),
            Helpers.CreateAllowedHeader(mockConfig),
        };
        var requestHeaders = Helpers.GetMergedHeaders(__requestHeaders_ignored, __requestHeaders_allowed);

        // Act
        var result = business.GetReplicatedHeaders(requestHeaders);

        // Assert
        Assert.Equal(requestHeaders.Count, result.Count);
        Assert.True(Helpers.AreHeaderDictionariesEqual(requestHeaders, result));
    }

    [Fact]
    public void GetReplicatedHeaders_ShouldReturnMatchedHeaders_WhenAllowAllIsFalse()
    {
        // Arrange
        var mockLogger = Mocked.GetLogger<HeaderReplicationBusiness>();
        var mockConfig = Mocked.GetHeaderReplicatorConfigurationWithDefaults(false);
        var business = new HeaderReplicationBusiness(mockConfig, mockLogger);

        // Arrange - Request Headers
        var __requestHeaders_ignored = new HeaderDictionary
        {
            Helpers.CreateIgnoredHeader(mockConfig),
            Helpers.CreateIgnoredHeader(mockConfig),
            Helpers.CreateIgnoredHeader(mockConfig),
        };
        var __requestHeaders_allowed = new HeaderDictionary
        {
            Helpers.CreateAllowedHeader(mockConfig),
            Helpers.CreateAllowedHeader(mockConfig),
            Helpers.CreateAllowedHeader(mockConfig),
        };
        var requestHeaders = Helpers.GetMergedHeaders(__requestHeaders_ignored, __requestHeaders_allowed);

        // Act
        var result = business.GetReplicatedHeaders(requestHeaders);

        // Assert
        foreach (var header in result)
        {
            if (__requestHeaders_ignored.ContainsKey(header.Key))
                Assert.Equal(HeaderReplicationBusiness.RedactedValue, header.Value);
            else
                Assert.Equal(__requestHeaders_allowed[header.Key], header.Value);
        }
    }

    [Fact]
    public void GetReplicatedHeaders_ShouldReturnMatchedHeaders_WhenAllowAllIsFalseAndIgnoredSentencesAreProvided()
    {
        // Arrange
        var mockLogger = Mocked.GetLogger<HeaderReplicationBusiness>();
        var mockConfig = Mocked.GetHeaderReplicatorConfigurationWithDefaults(false);
        var business = new HeaderReplicationBusiness(mockConfig, mockLogger);

        // Arrange - Request Headers
        var __requestHeaders_ignored = new HeaderDictionary
        {
            Helpers.CreateIgnoredHeader(mockConfig),
            Helpers.CreateIgnoredHeader(mockConfig),
            Helpers.CreateIgnoredHeader(mockConfig),
        };
        var __requestHeaders_allowed = new HeaderDictionary
        {
            Helpers.CreateAllowedHeader(mockConfig),
            Helpers.CreateAllowedHeader(mockConfig),
            Helpers.CreateAllowedHeader(mockConfig),
        };
        var requestHeaders = Helpers.GetMergedHeaders(__requestHeaders_ignored, __requestHeaders_allowed);

        // Act
        var result = business.GetReplicatedHeaders(requestHeaders);

        // Assert
        foreach (var header in result)
        {
            if (__requestHeaders_ignored.ContainsKey(header.Key))
                Assert.Equal(HeaderReplicationBusiness.RedactedValue, header.Value);
            else
                Assert.Equal(__requestHeaders_allowed[header.Key], header.Value);
        }
    }



    // Add some combined headers. These header keys will be ignored because they contain IgnoredHeaderSentence even if they start with AllowedHeaderPrefix
    // for (int i = 0; i < toBeAddedCombinedHeader; i++)
    //     __requestHeaders_ignored.Add($"{Helpers.GetAllowedHeaderPrefix(mockConfig)}-{Helpers.GetIgnoredHeaderSentence(mockConfig)}-combined-{i}", GetRandomValue());
    // [Fact]
    // public void Ctor_Should_ThrowArgumentNullException_WhenIgnoredSentencesIsNull()
    // {
    //     // Arrange
    //     IEnumerable<string> allowedPrefixes = GetEmptyEnumerable();
    //     IEnumerable<string> ignoredSentences = null;

    //     // Act
    //     Action act = () => new HeaderReplicationBusiness(false, allowedPrefixes, ignoredSentences);

    //     // Assert
    //     Assert.Throws<ArgumentNullException>(act);
    // }


    // [Fact]
    // public void Ctor_Should_ReturnEmptyDictionary_WhenRequestHeadersIsEmpty()
    // {
    //     // Arrange
    //     IEnumerable<string> allowedPrefixes = new List<string> { "X-", "Y-", "Z-" };
    //     IEnumerable<string> ignoredSentences = GetEmptyEnumerable();

    //     var business = new HeaderReplicationBusiness(false, allowedPrefixes, ignoredSentences);
    //     var requestHeaders = new HeaderDictionary();

    //     // Act
    //     var result = business.GetReplicatedHeaders(requestHeaders);

    //     // Assert
    //     Assert.Empty(result);
    // }

    // [Fact]
    // public void Ctor_Should_ReturnAllowedHeaders_WhenAllowedPrefixesAreProvided()
    // {
    //     // Arrange
    //     var allowedPrefixes = new List<string> { "X-", "Y-", "Z-" };
    //     var ignoredSentences = GetEmptyEnumerable();

    //     var business = new HeaderReplicationBusiness(false, allowedPrefixes, ignoredSentences);
    //     var requestHeaders = new HeaderDictionary
    //     {
    //         { "A-Test-Header", GetRandomValue() },
    //         { "B-Test-Header", GetRandomValue() },
    //         { "C-Test-Header", GetRandomValue() }
    //     };
    //     foreach (var prefix in allowedPrefixes)
    //         requestHeaders.Add(prefix + "Test-Header", GetRandomValue());

    //     // Act
    //     var responseHeaders = business.GetReplicatedHeaders(requestHeaders);

    //     // Assert
    //     Assert.Equal(allowedPrefixes.Count, responseHeaders.Count);
    //     foreach (var header in responseHeaders)
    //     {
    //         Assert.Contains(allowedPrefixes, header.Key.StartsWith);
    //     }
    // }


    // [Fact]
    // public void Ctor_Should_ReturnMatchedHeaders_WhenAllowedPrefixesAndIgnoredSentencesAreProvided()
    // {
    //     // Arrange
    //     var allowedPrefixes = new List<string> { "J-", "K-", "L-", "M-" };
    //     var ignoredSentences = new List<string> { "Test" };

    //     var business = new HeaderReplicationBusiness(false, allowedPrefixes, ignoredSentences);
    //     var requestHeaders = new HeaderDictionary
    //     {
    //         { $"A-{ignoredSentences.First()}-Header", GetRandomValue() },
    //         { $"B-{ignoredSentences.First()}-Header", GetRandomValue() },
    //         { $"C-{ignoredSentences.First()}-Header", GetRandomValue() },
    //     };
    //     foreach (var prefix in allowedPrefixes)
    //         requestHeaders.Add(prefix + "Header", GetRandomValue());

    //     var _ignoredList = requestHeaders.Where(h => ignoredSentences.Any(h.Key.Contains)).ToList();
    //     var _allowedList = requestHeaders.Except(_ignoredList).ToList();

    //     // Act
    //     var responseHeaders = business.GetReplicatedHeaders(requestHeaders);

    //     // Assert
    //     Assert.Equal(_allowedList.Count, responseHeaders.Count);
    //     foreach (var header in responseHeaders)
    //     {
    //         Assert.Contains(allowedPrefixes, header.Key.StartsWith);
    //         Assert.DoesNotContain(ignoredSentences, header.Key.Contains);
    //     }
    // }
}
