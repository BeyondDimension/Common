namespace System.Runtime.Serialization.Formatters;

/// <summary>
/// 对类型 <see cref="Cookie"/>, <see cref="CookieCollection"/>, <see cref="CookieContainer"/> 的序列化与反序列化实现
/// </summary>
public sealed class CookieFormatter :
    IMessagePackFormatter<Cookie?>,
    IMessagePackFormatter<CookieCollection?>,
    IMessagePackFormatter<CookieContainer?>,
    IMemoryPackFormatter<Cookie?>,
    IMemoryPackFormatter<CookieCollection?>,
    IMemoryPackFormatter<CookieContainer?>
{
    // https://github.com/neuecc/MessagePack-CSharp/blob/v2.4.59/src/MessagePack.UnityClient/Assets/Scripts/MessagePack/Formatters/CollectionFormatter.cs
    // https://github.com/Cysharp/MemoryPack/blob/1.9.12/src/MemoryPack.Core/Formatters/CollectionFormatters.cs

    // https://www.coderbusy.com/archives/2002.html

    public static readonly CookieFormatter Default = new();

    void IMessagePackFormatter<Cookie?>.Serialize(ref MessagePackWriter writer, Cookie? value, MessagePackSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNil();
        }
        else
        {
            CookieMessagePackObject mpo = value;
            MessagePackSerializer.Serialize(ref writer, mpo, options);
        }
    }

    Cookie? IMessagePackFormatter<Cookie?>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return default;
        }
        else
        {
            var dto = MessagePackSerializer.Deserialize<CookieMessagePackObject>(ref reader, options);
            return dto;
        }
    }

    void IMessagePackFormatter<CookieCollection?>.Serialize(ref MessagePackWriter writer, CookieCollection? value, MessagePackSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNil();
        }
        else
        {
            IMessagePackFormatter<Cookie?> formatter = this;
            IReadOnlyCollection<Cookie> cookies = value;
            writer.WriteArrayHeader(cookies.Count);
            foreach (Cookie cookie in cookies)
            {
                writer.CancellationToken.ThrowIfCancellationRequested();
                formatter.Serialize(ref writer, cookie, options);
            }
        }
    }

    CookieCollection? IMessagePackFormatter<CookieCollection?>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return default;
        }
        else
        {
            IMessagePackFormatter<Cookie?> formatter = this;
            var len = reader.ReadArrayHeader();
            var cookieCollection = new CookieCollection();
            options.Security.DepthStep(ref reader);
            try
            {
                for (int i = 0; i < len; i++)
                {
                    reader.CancellationToken.ThrowIfCancellationRequested();
                    var cookie = formatter.Deserialize(ref reader, options);
                    if (cookie != null) cookieCollection.Add(cookie);
                }
            }
            finally
            {
                reader.Depth--;
            }
            return cookieCollection;
        }
    }

    void IMessagePackFormatter<CookieContainer?>.Serialize(ref MessagePackWriter writer, CookieContainer? value, MessagePackSerializerOptions options)
    {
        CookieCollection? cookies = value?.GetAllCookies();
        IMessagePackFormatter<CookieCollection?> formatter = this;
        formatter.Serialize(ref writer, cookies, options);
    }

    CookieContainer? IMessagePackFormatter<CookieContainer?>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return default;
        }
        else
        {
            IMessagePackFormatter<Cookie?> formatter = this;
            var len = reader.ReadArrayHeader();
            var container = new CookieContainer();
            options.Security.DepthStep(ref reader);
            try
            {
                for (int i = 0; i < len; i++)
                {
                    reader.CancellationToken.ThrowIfCancellationRequested();
                    var cookie = formatter.Deserialize(ref reader, options);
                    if (cookie != null) container.Add(cookie);
                }
            }
            finally
            {
                reader.Depth--;
            }
            return container;
        }
    }

    void IMemoryPackFormatter<Cookie?>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Cookie? value)
    {
        if (value == null)
        {
            writer.WriteNullObjectHeader();
        }
        else
        {
            CookieMemoryPackable packable = value;
            writer.WritePackable(packable);
        }
    }

    void IMemoryPackFormatter<Cookie?>.Deserialize(ref MemoryPackReader reader, scoped ref Cookie? value)
    {
        if (reader.PeekIsNull())
        {
            value = null;
        }
        else
        {
            var wrapped = reader.ReadPackable<CookieMemoryPackable>();
            value = wrapped.Cookie;
        }
    }

    void IMemoryPackFormatter<CookieCollection?>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref CookieCollection? value)
    {
        if (value == null)
        {
            writer.WriteNullCollectionHeader();
        }
        else
        {
            IMemoryPackFormatter<Cookie?> formatter = this;
            IReadOnlyCollection<Cookie> cookies = value;
            writer.WriteCollectionHeader(cookies.Count);
            foreach (Cookie cookie in cookies)
            {
                var v = cookie;
                formatter.Serialize(ref writer, ref v);
            }
        }
    }

    void IMemoryPackFormatter<CookieCollection?>.Deserialize(ref MemoryPackReader reader, scoped ref CookieCollection? value)
    {
        if (!reader.TryReadCollectionHeader(out var length))
        {
            value = null;
        }
        else
        {
            if (value == null)
            {
                value = new CookieCollection();
            }
            else
            {
                value.Clear();
            }

            IMemoryPackFormatter<Cookie?> formatter = this;
            for (int i = 0; i < length; i++)
            {
                Cookie? v = default;
                formatter.Deserialize(ref reader, ref v);
                if (v != default) value.Add(v);
            }
        }
    }

    void IMemoryPackFormatter<CookieContainer?>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref CookieContainer? value)
    {
        CookieCollection? cookies = value?.GetAllCookies();
        IMemoryPackFormatter<CookieCollection?> formatter = this;
        formatter.Serialize(ref writer, ref cookies);
    }

    void IMemoryPackFormatter<CookieContainer?>.Deserialize(ref MemoryPackReader reader, scoped ref CookieContainer? value)
    {
        if (!reader.TryReadCollectionHeader(out var length))
        {
            value = null;
        }
        else
        {
            value ??= new CookieContainer();

            IMemoryPackFormatter<Cookie?> formatter = this;
            for (int i = 0; i < length; i++)
            {
                Cookie? v = default;
                formatter.Deserialize(ref reader, ref v);
                if (v != default) value.Add(v);
            }
        }
    }
}

[MessagePackObject]
public sealed class CookieMessagePackObject
{
    [SerializationConstructor]
    public CookieMessagePackObject()
    {

    }

    public CookieMessagePackObject(Cookie cookie)
    {
        Comment = cookie.Comment;
        CommentUri = cookie.CommentUri;
        Discard = cookie.Discard;
        Domain = cookie.Domain;
        Expires = cookie.Expires;
        HttpOnly = cookie.HttpOnly;
        Name = cookie.Name;
        Path = cookie.Path;
        Value = cookie.Value;
        Port = cookie.Port;
        Secure = cookie.Secure;
        Value = cookie.Value;
        Version = cookie.Version;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Cookie ToCookie()
    {
        Cookie cookie = new(Name, Value, Path, Domain)
        {
            Comment = Comment,
            CommentUri = CommentUri,
            Discard = Discard,
            Expires = Expires,
            HttpOnly = HttpOnly,
            Secure = Secure,
            Version = Version,
        };
        if (!string.IsNullOrEmpty(Port))
        {
            // 对 Port 的 set 会导致 m_port_implicit = false;
            // 默认值为 string.Empty
            cookie.Port = Port;
        }
        return cookie;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Cookie(CookieMessagePackObject cookie)
        => cookie.ToCookie();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CookieMessagePackObject(Cookie cookie)
        => new(cookie);

    /// <inheritdoc cref="Cookie.Comment"/>
    [MPKey(0)]
    public string Comment { get; set; } = "";

    /// <inheritdoc cref="Cookie.CommentUri"/>
    [MPKey(1)]
    public Uri? CommentUri { get; set; }

    /// <inheritdoc cref="Cookie.Discard"/>
    [MPKey(2)]
    public bool Discard { get; set; }

    /// <inheritdoc cref="Cookie.Domain"/>
    [MPKey(3)]
    public string Domain { get; set; } = null!;

    /// <inheritdoc cref="Cookie.Expires"/>
    [MPKey(5)]
    public DateTime Expires { get; set; }

    /// <inheritdoc cref="Cookie.HttpOnly"/>
    [MPKey(6)]
    public bool HttpOnly { get; set; }

    /// <inheritdoc cref="Cookie.Name"/>
    [MPKey(7)]
    public string Name { get; set; } = null!;

    /// <inheritdoc cref="Cookie.Path"/>
    [MPKey(8)]
    public string Path { get; set; } = "/";

    /// <inheritdoc cref="Cookie.Port"/>
    [MPKey(9)]
    public string Port { get; set; } = "";

    /// <inheritdoc cref="Cookie.Secure"/>
    [MPKey(10)]
    public bool Secure { get; set; }

    /// <inheritdoc cref="Cookie.Value"/>
    [MPKey(11)]
    public string Value { get; set; } = null!;

    /// <inheritdoc cref="Cookie.Version"/>
    [MPKey(12)]
    public int Version { get; set; }
}

[MemoryPackable]
public readonly partial struct CookieMemoryPackable
{
    [MemoryPackIgnore]
    public readonly Cookie Cookie;

    [MemoryPackInclude]
    string Comment => Cookie.Comment;

    [MemoryPackInclude]
    Uri? CommentUri => Cookie.CommentUri;

    [MemoryPackInclude]
    bool Discard => Cookie.Discard;

    [MemoryPackInclude]
    string Domain => Cookie.Domain;

    [MemoryPackInclude]
    DateTime Expires => Cookie.Expires;

    [MemoryPackInclude]
    bool HttpOnly => Cookie.HttpOnly;

    [MemoryPackInclude]
    string Name => Cookie.Name;

    [MemoryPackInclude]
    string Path => Cookie.Path;

    [MemoryPackInclude]
    string Port => Cookie.Port;

    [MemoryPackInclude]
    bool Secure => Cookie.Secure;

    [MemoryPackInclude]
    string Value => Cookie.Value;

    [MemoryPackInclude]
    int Version => Cookie.Version;

    [MemoryPackConstructor]
    CookieMemoryPackable(string? comment, Uri? commentUri, bool discard, string domain, DateTime expires, bool httpOnly, string name, string? path, string? port, bool secure, string? value, int version)
    {
        Cookie = new(name, value, path, domain)
        {
            Comment = comment,
            CommentUri = commentUri,
            Discard = discard,
            Expires = expires,
            HttpOnly = httpOnly,
            Secure = secure,
            Version = version,
        };
        if (!string.IsNullOrEmpty(port))
        {
            // 对 Port 的 set 会导致 m_port_implicit = false;
            // 默认值为 string.Empty
            Cookie.Port = port;
        }
    }

    public CookieMemoryPackable(Cookie cookie) => Cookie = cookie;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Cookie(CookieMemoryPackable value) => value.Cookie;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CookieMemoryPackable(Cookie value) => new(value);
}

public sealed class CookieFormatterAttribute : MemoryPackCustomFormatterAttribute<CookieFormatter, Cookie?>
{
    public sealed override CookieFormatter GetFormatter() => CookieFormatter.Default;

    public sealed class Formatter : MemoryPackFormatter<Cookie?>
    {
        public static readonly Formatter Default = new();

        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Cookie? value)
        {
            IMemoryPackFormatter<Cookie?> f = CookieFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref Cookie? value)
        {
            IMemoryPackFormatter<Cookie?> f = CookieFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}

public sealed class CookieCollectionFormatterAttribute : MemoryPackCustomFormatterAttribute<CookieFormatter, CookieCollection?>
{
    public sealed override CookieFormatter GetFormatter() => CookieFormatter.Default;

    public sealed class Formatter : MemoryPackFormatter<CookieCollection?>
    {
        public static readonly Formatter Default = new();

        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref CookieCollection? value)
        {
            IMemoryPackFormatter<CookieCollection?> f = CookieFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref CookieCollection? value)
        {
            IMemoryPackFormatter<CookieCollection?> f = CookieFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}

public sealed class CookieContainerFormatterAttribute : MemoryPackCustomFormatterAttribute<CookieFormatter, CookieContainer?>
{
    public sealed override CookieFormatter GetFormatter() => CookieFormatter.Default;

    public sealed class Formatter : MemoryPackFormatter<CookieContainer?>
    {
        public static readonly Formatter Default = new();

        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref CookieContainer? value)
        {
            IMemoryPackFormatter<CookieContainer?> f = CookieFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref CookieContainer? value)
        {
            IMemoryPackFormatter<CookieContainer?> f = CookieFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}