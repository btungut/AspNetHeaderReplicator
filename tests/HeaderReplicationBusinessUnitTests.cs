using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace DotNetHeaderReplicator.Tests;

public class HeaderReplicationBusinessUnitTests
{
    private IEnumerable<string> GetEmptyEnumerable() => Enumerable.Empty<string>();
    private StringValues GetRandomValue() => new StringValues(Guid.NewGuid().ToString());


    [Fact]
    public void HeaderReplicationBusiness_Should_ThrowArgumentNullException_WhenAllowedPrefixesIsNull()
    {
        // Arrange
        IEnumerable<string> allowedPrefixes = null;
        IEnumerable<string> ignoredSentences = GetEmptyEnumerable();

        // Act
        Action act = () => new HeaderReplicationBusiness(false, allowedPrefixes, ignoredSentences);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }
    

    [Fact]
    public void HeaderReplicationBusiness_Should_ThrowArgumentNullException_WhenIgnoredSentencesIsNull()
    {
        // Arrange
        IEnumerable<string> allowedPrefixes = GetEmptyEnumerable();
        IEnumerable<string> ignoredSentences = null;

        // Act
        Action act = () => new HeaderReplicationBusiness(false, allowedPrefixes, ignoredSentences);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }


    [Fact]
    public void HeaderReplicationBusiness_Should_ReturnEmptyDictionary_WhenRequestHeadersIsEmpty()
    {
        // Arrange
        IEnumerable<string> allowedPrefixes = new List<string> { "X-", "Y-", "Z-" };
        IEnumerable<string> ignoredSentences = GetEmptyEnumerable();

        var business = new HeaderReplicationBusiness(false, allowedPrefixes, ignoredSentences);
        var requestHeaders = new HeaderDictionary();

        // Act
        var result = business.GetReplicatedHeaders(requestHeaders);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void HeaderReplicationBusiness_Should_ReturnAllowedHeaders_WhenAllowedPrefixesAreProvided()
    {
        // Arrange
        var allowedPrefixes = new List<string> { "X-", "Y-", "Z-" };
        var ignoredSentences = GetEmptyEnumerable();

        var business = new HeaderReplicationBusiness(false, allowedPrefixes, ignoredSentences);
        var requestHeaders = new HeaderDictionary
        {
            { "A-Test-Header", GetRandomValue() },
            { "B-Test-Header", GetRandomValue() },
            { "C-Test-Header", GetRandomValue() }
        };
        foreach (var prefix in allowedPrefixes)
            requestHeaders.Add(prefix + "Test-Header", GetRandomValue());

        // Act
        var responseHeaders = business.GetReplicatedHeaders(requestHeaders);

        // Assert
        Assert.Equal(allowedPrefixes.Count, responseHeaders.Count);
        foreach (var header in responseHeaders)
        {
            Assert.Contains(allowedPrefixes, header.Key.StartsWith);
        }
    }


    [Fact]
    public void HeaderReplicationBusiness_Should_ReturnMatchedHeaders_WhenAllowedPrefixesAndIgnoredSentencesAreProvided()
    {
        // Arrange
        var allowedPrefixes = new List<string> { "J-", "K-", "L-", "M-" };
        var ignoredSentences = new List<string> { "Test" };

        var business = new HeaderReplicationBusiness(false, allowedPrefixes, ignoredSentences);
        var requestHeaders = new HeaderDictionary
        {
            { $"A-{ignoredSentences.First()}-Header", GetRandomValue() },
            { $"B-{ignoredSentences.First()}-Header", GetRandomValue() },
            { $"C-{ignoredSentences.First()}-Header", GetRandomValue() },
        };
        foreach (var prefix in allowedPrefixes)
            requestHeaders.Add(prefix + "Header", GetRandomValue());

        var _ignoredList = requestHeaders.Where(h => ignoredSentences.Any(h.Key.Contains)).ToList();
        var _allowedList = requestHeaders.Except(_ignoredList).ToList();

        // Act
        var responseHeaders = business.GetReplicatedHeaders(requestHeaders);

        // Assert
        Assert.Equal(_allowedList.Count, responseHeaders.Count);
        foreach (var header in responseHeaders)
        {
            Assert.Contains(allowedPrefixes, header.Key.StartsWith);
            Assert.DoesNotContain(ignoredSentences, header.Key.Contains);
        }
    }
}
