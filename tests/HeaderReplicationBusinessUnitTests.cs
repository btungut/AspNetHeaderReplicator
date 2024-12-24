using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace DotNetHeaderReplicator.Tests;

public class HeaderReplicationBusinessUnitTests
{
    private IEnumerable<string> GetEmptyEnumerable() => Enumerable.Empty<string>();
    

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
    public void HeaderReplicationBusiness_Should_ThrowArgumentNullException_WhenRequestHeadersIsNull()
    {
        // Arrange
        var business = new HeaderReplicationBusiness(false, GetEmptyEnumerable(), GetEmptyEnumerable());
        IHeaderDictionary requestHeaders = null;

        // Act
        Action act = () => business.GetReplicatedHeaders(requestHeaders);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }


    [Fact]
    public void HeaderReplicationBusiness_Should_ReturnEmptyDictionary_WhenRequestHeadersIsEmpty()
    {
        // Arrange
        IEnumerable<string> allowedPrefixes = GetEmptyEnumerable();
        IEnumerable<string> ignoredSentences = GetEmptyEnumerable();

        var business = new HeaderReplicationBusiness(false, allowedPrefixes, ignoredSentences);
        var requestHeaders = new HeaderDictionary();

        // Act
        var result = business.GetReplicatedHeaders(requestHeaders);

        // Assert
        Assert.Empty(result);
    }
}
