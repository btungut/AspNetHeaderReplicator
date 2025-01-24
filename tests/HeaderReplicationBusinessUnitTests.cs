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

        // Act
        Action act = () => new HeaderReplicationBusiness(null);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Ctor_ShouldThrowException_WhenConfigIgnoredHeaderSentencesIsNull()
    {
        // Arrange
        var mockConfig = Mocked.GetHeaderReplicatorConfiguration(false, Helpers.GetEmptyEnumerable(), null);

        // Act
        Action act = () => new HeaderReplicationBusiness(mockConfig);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Ctor_ShouldThrowException_WhenConfigAllowedHeaderPrefixesIsNull()
    {
        // Arrange
        var mockConfig = Mocked.GetHeaderReplicatorConfiguration(false, null, Helpers.GetEmptyEnumerable());

        // Act
        Action act = () => new HeaderReplicationBusiness(mockConfig);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void GetReplicatedHeaders_ShouldThrowException_WhenRequestHeadersIsNull()
    {
        // Arrange
        var mockConfig = Mocked.GetHeaderReplicatorConfiguration(false, Helpers.GetEmptyEnumerable(), Helpers.GetEmptyEnumerable());
        var business = new HeaderReplicationBusiness(mockConfig);

        // Act
        Action act = () => business.GetReplicatedHeaders(null);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void GetReplicatedHeaders_ShouldReturnEmptyDictionary_WhenRequestHeadersIsEmpty()
    {
        // Arrange
        var mockConfig = Mocked.GetHeaderReplicatorConfiguration(false, Helpers.GetEmptyEnumerable(), Helpers.GetEmptyEnumerable());
        var business = new HeaderReplicationBusiness(mockConfig);
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
        var mockConfig = Mocked.GetHeaderReplicatorConfigurationWithDefaults(true);
        var business = new HeaderReplicationBusiness(mockConfig);

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
        var mockConfig = Mocked.GetHeaderReplicatorConfigurationWithDefaults(false);
        var business = new HeaderReplicationBusiness(mockConfig);

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
        var mockConfig = Mocked.GetHeaderReplicatorConfigurationWithDefaults(false);
        var business = new HeaderReplicationBusiness(mockConfig);

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
}
