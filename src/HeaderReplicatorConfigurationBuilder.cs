public class HeaderReplicatorConfigurationBuilder
{
    private static readonly string[] __def_allowedHeaderPrefixes = new[] { "X-", "My-", "Req-", "Trace-", "Debug", "Pass-" };
    private static readonly string[] __def_ignoredHeaderSentences = new[] { "auth", "credential", "token", "pass", "secret", "hash", "cert" };


    private bool _allowAll;
    private readonly HashSet<string> _allowedPrefixes;
    private readonly HashSet<string> _ignoredSentences;

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

    public HeaderReplicatorConfigurationBuilder AllowHeaderPrefixes(params string[] prefixes)
    {
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

    public HeaderReplicatorConfigurationBuilder IgnoreHeaderSentences(params string[] sentences)
    {
        foreach (var sentence in sentences)
            IgnoreHeaderSentence(sentence);

        return this;
    }

    public HeaderReplicatorConfigurationBuilder AllowAll()
    {
        _allowAll = true;
        _allowedPrefixes?.Clear();
        _ignoredSentences?.Clear();
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

    public HeaderReplicatorConfiguration Build() => new(_allowAll, _allowedPrefixes, _ignoredSentences);

    public class HeaderReplicatorConfiguration
    {
        public bool AllowAll { get; internal set; }
        public HashSet<string> AllowedHeaderPrefixes { get; internal set; }
        public HashSet<string> IgnoredHeaderSentences { get; internal set; }

        internal HeaderReplicatorConfiguration(bool allowAll, IEnumerable<string> allowedPrefixes, IEnumerable<string> ignoredSentences)
        {
            AllowAll = allowAll;

            if (AllowAll && (allowedPrefixes == null || ignoredSentences == null))
                throw new ArgumentNullException("The allowedPrefixes and ignoredSentences cannot be null when AllowAll is set to true.");

            AllowedHeaderPrefixes = AllowAll ? [] : new HashSet<string>(allowedPrefixes, StringComparer.OrdinalIgnoreCase);
            IgnoredHeaderSentences = AllowAll ? [] : new HashSet<string>(ignoredSentences, StringComparer.OrdinalIgnoreCase);
        }
    }
}