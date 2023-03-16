// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://github.com/dotnet/aspnetcore/blob/v8.0.0-preview.2.23153.2/src/Http/WebUtilities/src/QueryHelpers.cs
// https://github.com/dotnet/aspnetcore/blob/v8.0.0-preview.2.23153.2/src/Shared/QueryStringEnumerable.cs
// https://github.com/dotnet/aspnetcore/blob/v8.0.0-preview.2.23153.2/src/Http/WebUtilities/src/KeyValueAccumulator.cs

using System.Buffers;
using System.Runtime.Intrinsics;

namespace Microsoft.AspNetCore.WebUtilities;

/// <summary>
/// Provides methods for parsing and manipulating query strings.
/// </summary>
public static class QueryHelpers
{
    /// <summary>
    /// Append the given query key and value to the URI.
    /// </summary>
    /// <param name="uri">The base URI.</param>
    /// <param name="name">The name of the query key.</param>
    /// <param name="value">The query value.</param>
    /// <returns>The combined result.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="uri"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
    public static string AddQueryString(string uri, string name, string value)
    {
        ArgumentNullException.ThrowIfNull(uri);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(value);

        return AddQueryString(
            uri, new[] { new KeyValuePair<string, string?>(name, value) });
    }

    /// <summary>
    /// Append the given query keys and values to the URI.
    /// </summary>
    /// <param name="uri">The base URI.</param>
    /// <param name="queryString">A dictionary of query keys and values to append.</param>
    /// <returns>The combined result.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="uri"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="queryString"/> is <c>null</c>.</exception>
    public static string AddQueryString(string uri, IDictionary<string, string?> queryString)
    {
        ArgumentNullException.ThrowIfNull(uri);
        ArgumentNullException.ThrowIfNull(queryString);

        return AddQueryString(uri, (IEnumerable<KeyValuePair<string, string?>>)queryString);
    }

    /// <summary>
    /// Append the given query keys and values to the URI.
    /// </summary>
    /// <param name="uri">The base URI.</param>
    /// <param name="queryString">A collection of query names and values to append.</param>
    /// <returns>The combined result.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="uri"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="queryString"/> is <c>null</c>.</exception>
    public static string AddQueryString(string uri, IEnumerable<KeyValuePair<string, StringValues>> queryString)
    {
        ArgumentNullException.ThrowIfNull(uri);
        ArgumentNullException.ThrowIfNull(queryString);

        return AddQueryString(uri, queryString.SelectMany(kvp => kvp.Value, (kvp, v) => KeyValuePair.Create(kvp.Key, v)));
    }

    /// <summary>
    /// Append the given query keys and values to the URI.
    /// </summary>
    /// <param name="uri">The base URI.</param>
    /// <param name="queryString">A collection of name value query pairs to append.</param>
    /// <returns>The combined result.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="uri"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="queryString"/> is <c>null</c>.</exception>
    public static string AddQueryString(
        string uri,
        IEnumerable<KeyValuePair<string, string?>> queryString)
    {
        ArgumentNullException.ThrowIfNull(uri);
        ArgumentNullException.ThrowIfNull(queryString);

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
            sb.Append(UrlEncoder.Default.Encode(parameter.Key));
            sb.Append('=');
            sb.Append(UrlEncoder.Default.Encode(parameter.Value));
            hasQuery = true;
        }

        sb.Append(anchorText);
        return sb.ToString();
    }

    /// <summary>
    /// Parse a query string into its component key and value parts.
    /// </summary>
    /// <param name="queryString">The raw query string value, with or without the leading '?'.</param>
    /// <returns>A collection of parsed keys and values.</returns>
    public static Dictionary<string, StringValues> ParseQuery(string? queryString)
    {
        var result = ParseNullableQuery(queryString);

        if (result == null)
        {
            return new Dictionary<string, StringValues>();
        }

        return result;
    }

    /// <summary>
    /// Parse a query string into its component key and value parts.
    /// </summary>
    /// <param name="queryString">The raw query string value, with or without the leading '?'.</param>
    /// <returns>A collection of parsed keys and values, null if there are no entries.</returns>
    public static Dictionary<string, StringValues>? ParseNullableQuery(string? queryString)
    {
        var accumulator = default(KeyValueAccumulator);
        var enumerable = new QueryStringEnumerable(queryString);

        foreach (var pair in enumerable)
        {
            accumulator.Append(pair.DecodeName().ToString(), pair.DecodeValue().ToString());
        }

        if (!accumulator.HasValues)
        {
            return null;
        }

        return accumulator.GetResults();
    }
}

/// <summary>
/// An enumerable that can supply the name/value pairs from a URI query string.
/// </summary>
internal readonly struct QueryStringEnumerable
{
    private readonly ReadOnlyMemory<char> _queryString;

    /// <summary>
    /// Constructs an instance of <see cref="QueryStringEnumerable"/>.
    /// </summary>
    /// <param name="queryString">The query string.</param>
    public QueryStringEnumerable(string? queryString)
        : this(queryString.AsMemory())
    {
    }

    /// <summary>
    /// Constructs an instance of <see cref="QueryStringEnumerable"/>.
    /// </summary>
    /// <param name="queryString">The query string.</param>
    public QueryStringEnumerable(ReadOnlyMemory<char> queryString)
    {
        _queryString = queryString;
    }

    /// <summary>
    /// Retrieves an object that can iterate through the name/value pairs in the query string.
    /// </summary>
    /// <returns>An object that can iterate through the name/value pairs in the query string.</returns>
    public Enumerator GetEnumerator()
        => new Enumerator(_queryString);

    /// <summary>
    /// Represents a single name/value pair extracted from a query string during enumeration.
    /// </summary>
    public readonly struct EncodedNameValuePair
    {
        /// <summary>
        /// Gets the name from this name/value pair in its original encoded form.
        /// To get the decoded string, call <see cref="DecodeName"/>.
        /// </summary>
        public readonly ReadOnlyMemory<char> EncodedName { get; }

        /// <summary>
        /// Gets the value from this name/value pair in its original encoded form.
        /// To get the decoded string, call <see cref="DecodeValue"/>.
        /// </summary>
        public readonly ReadOnlyMemory<char> EncodedValue { get; }

        internal EncodedNameValuePair(ReadOnlyMemory<char> encodedName, ReadOnlyMemory<char> encodedValue)
        {
            EncodedName = encodedName;
            EncodedValue = encodedValue;
        }

        /// <summary>
        /// Decodes the name from this name/value pair.
        /// </summary>
        /// <returns>Characters representing the decoded name.</returns>
        public ReadOnlyMemory<char> DecodeName()
            => Decode(EncodedName);

        /// <summary>
        /// Decodes the value from this name/value pair.
        /// </summary>
        /// <returns>Characters representing the decoded value.</returns>
        public ReadOnlyMemory<char> DecodeValue()
            => Decode(EncodedValue);

        private static ReadOnlyMemory<char> Decode(ReadOnlyMemory<char> chars)
        {
            // If the value is short, it's cheap to check up front if it really needs decoding. If it doesn't,
            // then we can save some allocations.
            return chars.Length < 16 && chars.Span.IndexOfAny('%', '+') < 0
                ? chars
                : Uri.UnescapeDataString(SpanHelper.ReplacePlusWithSpace(chars.Span)).AsMemory();
        }
    }

    /// <summary>
    /// An enumerator that supplies the name/value pairs from a URI query string.
    /// </summary>
    public struct Enumerator
    {
        private ReadOnlyMemory<char> _query;

        internal Enumerator(ReadOnlyMemory<char> query)
        {
            Current = default;
            _query = query.IsEmpty || query.Span[0] != '?'
                ? query
                : query[1..];
        }

        /// <summary>
        /// Gets the currently referenced key/value pair in the query string being enumerated.
        /// </summary>
        public EncodedNameValuePair Current { get; private set; }

        /// <summary>
        /// Moves to the next key/value pair in the query string being enumerated.
        /// </summary>
        /// <returns>True if there is another key/value pair, otherwise false.</returns>
        public bool MoveNext()
        {
            while (!_query.IsEmpty)
            {
                // Chomp off the next segment
                ReadOnlyMemory<char> segment;
                var delimiterIndex = _query.Span.IndexOf('&');
                if (delimiterIndex >= 0)
                {
                    segment = _query[..delimiterIndex];
                    _query = _query[(delimiterIndex + 1)..];
                }
                else
                {
                    segment = _query;
                    _query = default;
                }

                // If it's nonempty, emit it
                var equalIndex = segment.Span.IndexOf('=');
                if (equalIndex >= 0)
                {
                    Current = new EncodedNameValuePair(
                        segment[..equalIndex],
                        segment[(equalIndex + 1)..]);
                    return true;
                }
                else if (!segment.IsEmpty)
                {
                    Current = new EncodedNameValuePair(segment, default);
                    return true;
                }
            }

            Current = default;
            return false;
        }
    }

    private static class SpanHelper
    {
        private static readonly SpanAction<char, IntPtr> s_replacePlusWithSpace = ReplacePlusWithSpaceCore;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string ReplacePlusWithSpace(ReadOnlySpan<char> span)
        {
            fixed (char* ptr = &MemoryMarshal.GetReference(span))
            {
                return string.Create(span.Length, (IntPtr)ptr, s_replacePlusWithSpace);
            }
        }

        private static unsafe void ReplacePlusWithSpaceCore(Span<char> buffer, IntPtr state)
        {
            fixed (char* ptr = &MemoryMarshal.GetReference(buffer))
            {
                var input = (ushort*)state.ToPointer();
                var output = (ushort*)ptr;

                var i = (nint)0;
                var n = (nint)(uint)buffer.Length;

                if (Vector256.IsHardwareAccelerated && n >= Vector256<ushort>.Count)
                {
                    var vecPlus = Vector256.Create((ushort)'+');
                    var vecSpace = Vector256.Create((ushort)' ');

                    do
                    {
                        var vec = Vector256.Load(input + i);
                        var mask = Vector256.Equals(vec, vecPlus);
                        var res = Vector256.ConditionalSelect(mask, vecSpace, vec);
                        res.Store(output + i);
                        i += Vector256<ushort>.Count;
                    } while (i <= n - Vector256<ushort>.Count);
                }

                if (Vector128.IsHardwareAccelerated && n - i >= Vector128<ushort>.Count)
                {
                    var vecPlus = Vector128.Create((ushort)'+');
                    var vecSpace = Vector128.Create((ushort)' ');

                    do
                    {
                        var vec = Vector128.Load(input + i);
                        var mask = Vector128.Equals(vec, vecPlus);
                        var res = Vector128.ConditionalSelect(mask, vecSpace, vec);
                        res.Store(output + i);
                        i += Vector128<ushort>.Count;
                    } while (i <= n - Vector128<ushort>.Count);
                }

                for (; i < n; ++i)
                {
                    if (input[i] != '+')
                    {
                        output[i] = input[i];
                    }
                    else
                    {
                        output[i] = ' ';
                    }
                }
            }
        }
    }
}

/// <summary>
/// This API supports infrastructure and is not intended to be used
/// directly from your code. This API may change or be removed in future releases.
/// </summary>
internal struct KeyValueAccumulator
{
    private Dictionary<string, StringValues> _accumulator;
    private Dictionary<string, List<string>> _expandingAccumulator;

    /// <summary>
    /// This API supports infrastructure and is not intended to be used
    /// directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public void Append(string key, string value)
    {
        if (_accumulator == null)
        {
            _accumulator = new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase);
        }

        StringValues values;
        if (_accumulator.TryGetValue(key, out values))
        {
            if (values.Count == 0)
            {
                // Marker entry for this key to indicate entry already in expanding list dictionary
                _expandingAccumulator[key].Add(value);
            }
            else if (values.Count == 1)
            {
                // Second value for this key
                _accumulator[key] = new string[] { values[0]!, value };
            }
            else
            {
                // Third value for this key
                // Add zero count entry and move to data to expanding list dictionary
                _accumulator[key] = default(StringValues);

                if (_expandingAccumulator == null)
                {
                    _expandingAccumulator = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                }

                // Already 3 entries so use starting allocated as 8; then use List's expansion mechanism for more
                var list = new List<string>(8);
                var array = values.ToArray();

                list.Add(array[0]!);
                list.Add(array[1]!);
                list.Add(value);

                _expandingAccumulator[key] = list;
            }
        }
        else
        {
            // First value for this key
            _accumulator[key] = new StringValues(value);
        }

        ValueCount++;
    }

    /// <summary>
    /// This API supports infrastructure and is not intended to be used
    /// directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public bool HasValues => ValueCount > 0;

    /// <summary>
    /// This API supports infrastructure and is not intended to be used
    /// directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public int KeyCount => _accumulator?.Count ?? 0;

    /// <summary>
    /// This API supports infrastructure and is not intended to be used
    /// directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public int ValueCount { get; private set; }

    /// <summary>
    /// This API supports infrastructure and is not intended to be used
    /// directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public Dictionary<string, StringValues> GetResults()
    {
        if (_expandingAccumulator != null)
        {
            // Coalesce count 3+ multi-value entries into _accumulator dictionary
            foreach (var entry in _expandingAccumulator)
            {
                _accumulator[entry.Key] = new StringValues(entry.Value.ToArray());
            }
        }

        return _accumulator ?? new Dictionary<string, StringValues>(0, StringComparer.OrdinalIgnoreCase);
    }
}