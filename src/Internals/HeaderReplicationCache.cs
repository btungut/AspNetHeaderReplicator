namespace AspNetHeaderReplicator.Internals;

internal class HeaderReplicationCache
{
    #region Singleton pattern with Lazy implementation
    private static readonly Lazy<HeaderReplicationCache> _instance = new(() => new HeaderReplicationCache(), true);
    internal static HeaderReplicationCache Instance => _instance.Value;
    #endregion


    private readonly HashSet<string> _ignoredHeaders;
    private readonly HashSet<string> _allowedHeaders;
    internal HeaderReplicationCache()
    {
        _ignoredHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        _allowedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    internal bool IsIgnoredHeader(string header)
    {
        return _ignoredHeaders.Contains(header);
    }

    internal bool IsAllowedHeader(string header)
    {
        return _allowedHeaders.Contains(header);
    }

    internal void AddIgnoredHeader(string header)
    {
        lock (_ignoredHeaders)
        {
            _ignoredHeaders.Add(header);
        }
    }

    internal void AddAllowedHeader(string header)
    {
        lock (_allowedHeaders)
        {
            _allowedHeaders.Add(header);
        }
    }
}