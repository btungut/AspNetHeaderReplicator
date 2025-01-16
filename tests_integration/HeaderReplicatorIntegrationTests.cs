using Xunit;

namespace DotNetHeaderReplicator.IntegrationTests;

public class HeaderReplicatorIntegrationTests
{
    [Fact]
    public async Task Default_ShouldReplicate_HeadersWithDefaultConfiguration()
    {
        // Init
        using var client = (await IntegrationTestBuilder
            .CreateAsync(cfg =>
            {
                // default
            }))
            .GetClient();


        // Arrange
        var requestHeaders_allowed = new Dictionary<string, string>
        {
            { "X-My-Header", "MyValue" },
            { "X-Some-Infos", "SomeValue" }
        };
        var requestHeaders_redacted = new Dictionary<string, string>
        {
            { "X-Hello-Token-Here", Guid.NewGuid().ToString() },
            { "X-Auth-Key", Guid.NewGuid().ToString() },
            { "X-User-Pass", Guid.NewGuid().ToString() }
        };
        using var request = Helpers.CreateHttpRequestMessage(requestHeaders_allowed, requestHeaders_redacted);

        // Act
        using var response = await client.SendAsync(request);
        var responseHeaders = response.Headers.ToDictionary(h => h.Key, h => h.Value.FirstOrDefault());

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains(requestHeaders_allowed.Keys, responseHeaders.ContainsKey);
        Assert.Contains(requestHeaders_redacted.Keys, responseHeaders.ContainsKey);

        foreach (var header in requestHeaders_allowed)
            Assert.Equal(header.Value, responseHeaders[header.Key]);

        foreach (var header in requestHeaders_redacted)
            Assert.StartsWith("REDACTED", responseHeaders[header.Key], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AllowAll_ShouldReplicate_AllHeaders()
    {
        // Init
        using var client = (await IntegrationTestBuilder
            .CreateAsync(cfg =>
            {
                cfg.AllowAll();
            }))
            .GetClient();


        // Arrange
        var requestHeaders_allowed = new Dictionary<string, string>
        {
            { "X-My-Header", "MyValue" },
            { "X-Some-Infos", "SomeValue" }
        };
        var requestHeaders_redacted = new Dictionary<string, string>
        {
            { "X-Hello-Token-Here", Guid.NewGuid().ToString() },
            { "X-Auth-Key", Guid.NewGuid().ToString() },
            { "X-User-Pass", Guid.NewGuid().ToString() }
        };
        using var request = Helpers.CreateHttpRequestMessage(requestHeaders_allowed, requestHeaders_redacted);

        // Act
        using var response = await client.SendAsync(request);
        var responseHeaders = response.Headers.ToDictionary(h => h.Key, h => h.Value.FirstOrDefault());

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains(requestHeaders_allowed.Keys, responseHeaders.ContainsKey);
        Assert.Contains(requestHeaders_redacted.Keys, responseHeaders.ContainsKey);

        var merged = requestHeaders_allowed.Concat(requestHeaders_redacted);
        foreach (var header in merged)
            Assert.Equal(header.Value, responseHeaders[header.Key]);
    }
}