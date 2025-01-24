namespace AspNetHeaderReplicator.IntegrationTests;

internal static class Helpers
{
    public static KeyValuePair<string, object> AddRequestHeader(this HttpRequestMessage request, string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));

        request.Headers.Add(key, value);
        return new KeyValuePair<string, object>(key, value);
    }

    public static HttpRequestMessage CreateHttpRequestMessage(params IEnumerable<KeyValuePair<string, string>>[] headers)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/");
        foreach (var header in headers.SelectMany(h => h))
        {
            request.Headers.Add(header.Key, header.Value.ToString());
        }

        return request;
    }
}