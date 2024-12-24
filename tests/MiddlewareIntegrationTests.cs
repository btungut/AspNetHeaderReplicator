using Microsoft.AspNetCore.Http;

namespace DotNetHeaderReplicator.Tests;

public class MiddlewareIntegrationTests
{
    [Fact]
    public async Task Middleware_Should_ReplicateAllAllowedHeaders_WhenDefaultConfiguration()
    {
        // Arrange
        var client = IntegrationTestHelper.GetHttpClient(builder =>
        {
            // Do not modify the builder, let it use the default configuration
        });

        var headers = new Dictionary<string, string>();
        foreach (var defaultAllowedPrefix in HeaderReplicatorConfigurationBuilder.__def_allowedHeaderPrefixes)
            headers.Add(defaultAllowedPrefix + "Test-Header", Guid.NewGuid().ToString());


        // Act
        var response = await IntegrationTestHelper.GetResponseAsync(client, request =>
        {
            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);
        });

        // Assert
        foreach (var header in headers)
        {
            Assert.True(response.Headers.Contains(header.Key));
            Assert.Equal(header.Value, response.Headers.GetValues(header.Key).First());
        }
    }

    [Fact]
    public async Task Middleware_Should_ReplicateAllHeaders_WhenAllAllowed()
    {
        // Arrange
        var client = IntegrationTestHelper.GetHttpClient(builder =>
        {
            builder.AllowAll();
        });

        var headers = new Dictionary<string, string>();
        foreach (var defaultAllowedPrefix in HeaderReplicatorConfigurationBuilder.__def_allowedHeaderPrefixes)
            foreach (var defaultIgnoredSentence in HeaderReplicatorConfigurationBuilder.__def_ignoredHeaderSentences)
                headers.Add(defaultAllowedPrefix + defaultIgnoredSentence, Guid.NewGuid().ToString());

        // Act
        var response = await IntegrationTestHelper.GetResponseAsync(client, request =>
        {
            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);
        });

        // Assert
        foreach (var header in headers)
        {
            Assert.True(response.Headers.Contains(header.Key));
            Assert.Equal(header.Value, response.Headers.GetValues(header.Key).First());
        }
    }

    [Fact]
    public async Task Middleware_Should_ReplicateOnlyAllowedHeaders_WhenAllowHeaderPrefixesIsCalled()
    {
        var allowedPrefix = new string[] { "Burak-", "IndependentPrefix-", "SomePrefix-" };

        // Arrange
        var client = IntegrationTestHelper.GetHttpClient(builder =>
        {
            builder
                .ClearAll() // Clear all default configuration, both of AllowedPrefixes and IgnoredSentences
                .AllowHeaderPrefixes(allowedPrefix); // Allow only the given prefixes
        });

        var headers = new Dictionary<string, string>();
        foreach (var prefix in allowedPrefix)
            headers.Add(prefix + "Test-Header", Guid.NewGuid().ToString());
        var nonAllowedHeaders = new Dictionary<string, string>
        {
            { "X-Test-Header", Guid.NewGuid().ToString() },
            { "Y-Test-Header", Guid.NewGuid().ToString() },
            { "Z-Test-Header", Guid.NewGuid().ToString() }
        };

        // Act
        var response = await IntegrationTestHelper.GetResponseAsync(client, request =>
        {
            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);

            foreach (var header in nonAllowedHeaders)
                request.Headers.Add(header.Key, header.Value);
        });

        // Assert
        foreach (var header in response.Headers)
        {
            Assert.DoesNotContain(nonAllowedHeaders, non => header.Key.StartsWith(non.Key));
            Assert.Contains(allowedPrefix, _ => header.Key.StartsWith(_));
            Assert.True(response.Headers.Contains(header.Key));
        }
    }

    [Fact]
    public async Task Middleware_Should_RedactByDefaultIgnoredSentences_WhenDefaultConfiguration()
    {
        // Arrange
        var client = IntegrationTestHelper.GetHttpClient(builder =>
        {
            // Do not modify the builder, let it use the default configuration
        });

        var headers = new Dictionary<string, string>();
        foreach (var defaultIgnoredSentence in HeaderReplicatorConfigurationBuilder.__def_ignoredHeaderSentences)
            headers.Add("X-SomeKey-" + defaultIgnoredSentence, Guid.NewGuid().ToString());

        // Act
        var response = await IntegrationTestHelper.GetResponseAsync(client, request =>
        {
            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);
        });

        // Assert all response headers are redacted
        foreach (var header in response.Headers)
        {
            Assert.Equal(HeaderReplicationBusiness.RedactedValue, response.Headers.GetValues(header.Key).First());
        }
    }


    // [Fact]
    // public async Task Middleware_Should_ReplicateOnlyAllowedHeaders_WhenCustomConfiguration()
    // {
    //     var allowedPrefix = new string[] { "Burak-", "IndependentPrefix-", "SomePrefix-" };
    //     // Arrange
    //     var client = IntegrationTestHelper.GetHttpClient(builder =>
    //     {
    //         builder
    //             .ClearAll() // Clear all default configuration, both of AllowedPrefixes and IgnoredSentences
    //             .AllowHeaderPrefixes(allowedPrefix);
    //     });

    //     var headers = new Dictionary<string, string>();
    //     foreach (var prefix in allowedPrefix)
    //         headers.Add(prefix + "Test-Header", Guid.NewGuid().ToString());
    //     var nonAllowedHeaders = new Dictionary<string, string>
    //     {
    //         { "X-Test-Header", Guid.NewGuid().ToString() },
    //         { "Y-Test-Header", Guid.NewGuid().ToString() },
    //         { "Z-Test-Header", Guid.NewGuid().ToString() }
    //     };
    //     // append non-allowed headers into headers
    //     foreach (var header in nonAllowedHeaders)
    //         headers.Add(header.Key, header.Value);

    //     // Act
    //     var response = await IntegrationTestHelper.GetResponseAsync(client, request =>
    //     {
    //         foreach (var header in headers)
    //             request.Headers.Add(header.Key, header.Value);
    //     });

    //     // Assert

    //     foreach (var header in headers)
    //     {
    //         Assert.DoesNotContain(nonAllowedHeaders, non => non.Key == header.Key);
    //         Assert.Contains(allowedPrefix, prefix => header.Key.StartsWith(prefix));
    //         Assert.True(response.Headers.Contains(header.Key));
    //         Assert.Equal(header.Value, response.Headers.GetValues(header.Key).First());
    //     }

    // }
}