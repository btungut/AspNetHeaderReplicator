/// <summary>
/// Represents the configuration for the header replicator which is being created by the <see cref="HeaderReplicatorConfigurationBuilder"/> for the <see cref="HeaderReplicatorMiddleware"/>.
/// </summary>
namespace DotNetHeaderReplicator;

public interface IHeaderReplicatorConfiguration
{
    bool AllowAll { get; }
    IReadOnlyCollection<string> AllowedHeaderPrefixes { get; }
    IReadOnlyCollection<string> IgnoredHeaderSentences { get; }
}

internal class HeaderReplicatorConfiguration : IHeaderReplicatorConfiguration
{
    public bool AllowAll { get; internal set; }

    private readonly HashSet<string> _allowedHeaderPrefixes;
    public IReadOnlyCollection<string> AllowedHeaderPrefixes => _allowedHeaderPrefixes;

    private readonly HashSet<string> _ignoredHeaderSentences;
    public IReadOnlyCollection<string> IgnoredHeaderSentences => _ignoredHeaderSentences;

    internal HeaderReplicatorConfiguration(bool allowAll, HashSet<string> allowedPrefixes, HashSet<string> ignoredSentences)
    {
        AllowAll = allowAll;
        _allowedHeaderPrefixes = allowedPrefixes ?? throw new ArgumentNullException(nameof(allowedPrefixes));
        _ignoredHeaderSentences = ignoredSentences ?? throw new ArgumentNullException(nameof(ignoredSentences));

        if (AllowAll && (_allowedHeaderPrefixes.Count > 0 || _ignoredHeaderSentences.Count > 0))
            throw new InvalidOperationException("When AllowAll is true, the allowed prefixes and ignored sentences must be empty.");
    }
}