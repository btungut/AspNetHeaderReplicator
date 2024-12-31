/// <summary>
/// Represents a builder for <see cref="IHeaderReplicatorConfiguration"/>.
/// </summary>
namespace DotNetHeaderReplicator;

public class HeaderReplicatorConfigurationBuilder
{
    internal static readonly string[] __def_allowedHeaderPrefixes = new[] { "X-", "My-", "Req-", "Trace-", "Debug-", "Verbose-" };
    internal static readonly string[] __def_ignoredHeaderSentences = new[] { "auth", "credential", "token", "pass", "secret", "hash", "cert" };


    private bool _allowAll;
    private readonly HashSet<string> _allowedPrefixes;
    private readonly HashSet<string> _ignoredSentences;


    private HeaderReplicatorConfiguration _builtConfiguration;
    private static object _locker = new object();

    internal HeaderReplicatorConfigurationBuilder()
    {
        _allowAll = false;
        _allowedPrefixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        _allowedPrefixes.UnionWith(__def_allowedHeaderPrefixes);

        _ignoredSentences = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        _ignoredSentences.UnionWith(__def_ignoredHeaderSentences);
    }

    private void ThrowExceptionIfAllowAll()
    {
        if (_allowAll)
            throw new InvalidOperationException("This instance of HeaderReplicatorConfiguration is configured to allow all headers.");
    }

    public HeaderReplicatorConfigurationBuilder AllowHeaderPrefix(string prefix)
    {
        ThrowExceptionIfAllowAll();

        if (string.IsNullOrWhiteSpace(prefix))
            throw new ArgumentException("The prefix cannot be null or empty.", nameof(prefix));

        _allowedPrefixes.Add(prefix);
        return this;
    }

    public HeaderReplicatorConfigurationBuilder AllowHeaderPrefixes(IEnumerable<string> prefixes)
    {
        if (prefixes == null) throw new ArgumentNullException(nameof(prefixes));
        if (prefixes.Count() == 0) throw new ArgumentException("The prefixes cannot be empty.", nameof(prefixes));

        foreach (var prefix in prefixes)
            AllowHeaderPrefix(prefix);

        return this;
    }

    public HeaderReplicatorConfigurationBuilder IgnoreHeaderSentence(string sentence)
    {
        ThrowExceptionIfAllowAll();

        if (string.IsNullOrWhiteSpace(sentence))
            throw new ArgumentException("The sentence cannot be null or empty.", nameof(sentence));

        _ignoredSentences.Add(sentence);
        return this;
    }

    public HeaderReplicatorConfigurationBuilder IgnoreHeaderSentences(IEnumerable<string> sentences)
    {
        if (sentences == null) throw new ArgumentNullException(nameof(sentences));
        if (sentences.Count() == 0) throw new ArgumentException("The sentences cannot be empty.", nameof(sentences));

        foreach (var sentence in sentences)
            IgnoreHeaderSentence(sentence);

        return this;
    }

    public HeaderReplicatorConfigurationBuilder AllowAll()
    {
        _allowAll = true;
        _allowedPrefixes.Clear();
        _ignoredSentences.Clear();
        return this;
    }

    public HeaderReplicatorConfigurationBuilder ClearAll()
    {
        ThrowExceptionIfAllowAll();
        _allowedPrefixes.Clear();
        _ignoredSentences.Clear();
        return this;
    }

    public HeaderReplicatorConfigurationBuilder ClearAllowedHeaderPrefixes()
    {
        ThrowExceptionIfAllowAll();
        _allowedPrefixes.Clear();
        return this;
    }

    public HeaderReplicatorConfigurationBuilder ClearIgnoredHeaderSentences()
    {
        ThrowExceptionIfAllowAll();
        _ignoredSentences.Clear();
        return this;
    }

    public IHeaderReplicatorConfiguration Build()
    {
        Func<bool> isBuilt = () => _builtConfiguration != null;
        Action throwIfBuilt = () =>
        {
            if (isBuilt())
                throw new InvalidOperationException("This instance of HeaderReplicatorConfigurationBuilder has already been built.");
        };

        throwIfBuilt();

        lock (_locker)
        {
            throwIfBuilt();

            _builtConfiguration = new HeaderReplicatorConfiguration(_allowAll, _allowedPrefixes, _ignoredSentences);
            return _builtConfiguration;
        }
    }

}

