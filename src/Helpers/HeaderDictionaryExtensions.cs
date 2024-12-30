using Microsoft.AspNetCore.Http;

namespace DotNetHeaderReplicator;

internal static class HeaderDictionaryExtensions
{
    public static void AddOrReplaceRange(this IHeaderDictionary dic, IHeaderDictionary source)
    {
        if (dic == null) throw new ArgumentNullException(nameof(dic));
        if (source == null) throw new ArgumentNullException(nameof(source));

        foreach (var (key, value) in source)
        {
            dic[key] = value;
        }
    }
}