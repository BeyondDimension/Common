using System.Text;
using System.Text.Encodings.Web;

namespace System;

static partial class String2
{
    /// <summary>
    /// Append the given query keys and values to the URI.
    /// <para>https://github.com/dotnet/aspnetcore/blob/v9.0.5/src/Http/WebUtilities/src/QueryHelpers.cs#L76</para>
    /// </summary>
    /// <param name="uri">The base URI.</param>
    /// <param name="queryString">A collection of name value query pairs to append.</param>
    /// <returns>The combined result.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="uri"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="queryString"/> is <c>null</c>.</exception>
    public static string AddQueryString(
        string uri,
        params IEnumerable<KeyValuePair<string, string?>>? queryString)
    {
        ArgumentNullException.ThrowIfNull(uri);
        if (queryString == null)
        {
            return uri;
        }

        var anchorIndex = uri.IndexOf('#');
        var uriToBeAppended = uri.AsSpan();
        var anchorText = ReadOnlySpan<char>.Empty;
        // If there is an anchor, then the query string must be inserted before its first occurrence.
        if (anchorIndex != -1)
        {
            anchorText = uriToBeAppended[anchorIndex..];
            uriToBeAppended = uriToBeAppended[..anchorIndex];
        }

        var queryIndex = uriToBeAppended.IndexOf('?');
        var hasQuery = queryIndex != -1;

        var sb = new StringBuilder();
        sb.Append(uriToBeAppended);
        foreach (var parameter in queryString)
        {
            if (parameter.Value == null)
            {
                continue;
            }

            sb.Append(hasQuery ? '&' : '?');
            if (!string.IsNullOrEmpty(parameter.Key))
            {
                sb.Append(UrlEncoder.Default.Encode(parameter.Key));
            }
            sb.Append('=');
            if (!string.IsNullOrEmpty(parameter.Value))
            {
                sb.Append(UrlEncoder.Default.Encode(parameter.Value));
            }
            hasQuery = true;
        }

        sb.Append(anchorText);
        return sb.ToString();
    }
}
