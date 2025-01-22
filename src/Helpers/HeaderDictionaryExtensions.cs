/// <summary>
/// Represents extension methods for the <see cref="IHeaderDictionary"/> interface.
/// </summary>
using Microsoft.AspNetCore.Http;

namespace AspNetHeaderReplicator;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal static class HeaderDictionaryExtensions
{
    internal static void AddOrReplaceRange(this IHeaderDictionary dic, IHeaderDictionary source)
    {
        if (dic == null) throw new ArgumentNullException(nameof(dic));
        if (source == null) throw new ArgumentNullException(nameof(source));

        foreach (var (key, value) in source)
        {
            dic[key] = value;
        }
    }
}