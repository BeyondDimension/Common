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

    /// <summary>
    /// 默认的 <see cref="CookieFormatter"/> 实例
    /// </summary>
    public static readonly CookieFormatter Default = new();

    /// <summary>
    /// 对可空 <see cref="Cookie"/> 类型进行序列化
    /// </summary>
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

    /// <summary>
    /// 对可空 <see cref="Cookie"/> 类型进行反列化
    /// </summary>
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

    /// <summary>
    /// 对可空 <see cref="CookieCollection"/> 类型进行序列化
    /// </summary>
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

    /// <summary>
    /// 将可空 <see cref="CookieCollection"/> 类型进行反列化
    /// </summary>
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

    /// <summary>
    /// 将可空 <see cref="CookieContainer"/> 对象序列化为 <see cref="MessagePackWriter"/> 对象
    /// </summary>
    void IMessagePackFormatter<CookieContainer?>.Serialize(ref MessagePackWriter writer, CookieContainer? value, MessagePackSerializerOptions options)
    {
        CookieCollection? cookies = value?.GetAllCookies();
        IMessagePackFormatter<CookieCollection?> formatter = this;
        formatter.Serialize(ref writer, cookies, options);
    }

    /// <summary>
    /// 将 <see cref="MessagePackReader"/> 对象反序列化为可空 <see cref="CookieContainer"/> 对象
    /// </summary>
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

    /// <summary>
    /// 将可为空 <see cref="Cookie"/> 对象序列化为 <see cref="MemoryPackWriter{TBufferWriter}"/>
    /// </summary>
    /// <typeparam name="TBufferWriter"></typeparam>
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

    /// <summary>
    /// 将 <see cref="MemoryPackReader"/> 中的数据反序列化为可为空 <see cref="Cookie"/> 对象
    /// </summary>
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

    /// <summary>
    /// 将可为空 <see cref="CookieCollection"/> 对象序列化为 <see cref="MemoryPackWriter{TBufferWriter}"/>
    /// </summary>
    /// <typeparam name="TBufferWriter"></typeparam>
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

    /// <summary>
    /// 将 <see cref="MemoryPackReader"/> 中的数据反序列化为可为空 <see cref="CookieCollection"/> 对象
    /// </summary>
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
                value = [];
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

    /// <summary>
    /// 将可为空 <see cref="CookieContainer"/> 对象序列化为 <see cref="MemoryPackWriter{TBufferWriter}"/>
    /// </summary>
    /// <typeparam name="TBufferWriter"></typeparam>
    void IMemoryPackFormatter<CookieContainer?>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref CookieContainer? value)
    {
        CookieCollection? cookies = value?.GetAllCookies();
        IMemoryPackFormatter<CookieCollection?> formatter = this;
        formatter.Serialize(ref writer, ref cookies);
    }

    /// <summary>
    /// 将 <see cref="MemoryPackReader"/> 中的数据反序列化为可为空 <see cref="CookieContainer"/> 对象
    /// </summary>
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

/// <summary>
/// 表示 Cookie 消息包装对象
/// </summary>
[MessagePackObject]
public sealed class CookieMessagePackObject
{
    /// <summary>
    /// 初始化 <see cref="CookieMessagePackObject"/> 类的新实例
    /// </summary>
    [SerializationConstructor]
    public CookieMessagePackObject()
    {
    }

    /// <summary>
    /// 初始化 <see cref="CookieMessagePackObject"/> 类的新实例
    /// </summary>
    /// <param name="cookie"></param>
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

    /// <summary>
    /// 将 <see cref="CookieMessagePackObject"/> 转换为 <see cref="Cookie"/> 对象
    /// </summary>
    /// <returns>转换后的 <see cref="Cookie"/> 对象</returns>
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
    /// <inheritdoc cref="Cookie"/>
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

    /// <summary>
    /// 实例化 <see cref="CookieMemoryPackable.Cookie"/> 的值
    /// </summary>
    /// <param name="cookie"></param>
    public CookieMemoryPackable(Cookie cookie) => Cookie = cookie;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Cookie(CookieMemoryPackable value) => value.Cookie;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CookieMemoryPackable(Cookie value) => new(value);
}

/// <summary>
/// Cookie 格式化程序特性
/// </summary>
public sealed class CookieFormatterAttribute : MemoryPackCustomFormatterAttribute<CookieFormatter, Cookie?>
{
    /// <summary>
    /// 获取 <see cref="CookieFormatter.Default"/> 实例
    /// </summary>
    public sealed override CookieFormatter GetFormatter() => CookieFormatter.Default;

    /// <summary>
    /// 可空 <see cref="Cookie"/> 格式化程序
    /// </summary>
    public sealed class Formatter : MemoryPackFormatter<Cookie?>
    {
        /// <summary>
        /// 默认的 <see cref="Formatter"/> 实例
        /// </summary>
        public static readonly Formatter Default = new();

        /// <summary>
        /// 将可空 <see cref="Cookie"/> 对象序列化为 <see cref="MemoryPackWriter{TBufferWriter}"/> 对象
        /// </summary>
        /// <typeparam name="TBufferWriter"></typeparam>
        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref Cookie? value)
        {
            IMemoryPackFormatter<Cookie?> f = CookieFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        /// <summary>
        /// 从 <see cref="MemoryPackReader"/> 对象反序列化为可空 <see cref="Cookie"/> 对象
        /// </summary>
        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref Cookie? value)
        {
            IMemoryPackFormatter<Cookie?> f = CookieFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}

/// <summary>
/// CookieCollection 格式化程序特性
/// </summary>
public sealed class CookieCollectionFormatterAttribute : MemoryPackCustomFormatterAttribute<CookieFormatter, CookieCollection?>
{
    /// <summary>
    /// 获取 <see cref="CookieFormatter.Default"/> 实例
    /// </summary>
    public sealed override CookieFormatter GetFormatter() => CookieFormatter.Default;

    /// <summary>
    /// 可空 <see cref="CookieCollection"/> 格式化程序
    /// </summary>
    public sealed class Formatter : MemoryPackFormatter<CookieCollection?>
    {
        /// <summary>
        /// 默认的 <see cref="Formatter"/> 实例
        /// </summary>
        public static readonly Formatter Default = new();

        /// <summary>
        /// 将可空 <see cref="CookieCollection"/> 对象序列化为 <see cref="MemoryPackWriter{TBufferWriter}"/> 对象
        /// </summary>
        /// <typeparam name="TBufferWriter"></typeparam>
        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref CookieCollection? value)
        {
            IMemoryPackFormatter<CookieCollection?> f = CookieFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        /// <summary>
        /// 从 <see cref="MemoryPackReader"/> 对象反序列化为可空 <see cref="CookieCollection"/> 对象
        /// </summary>
        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref CookieCollection? value)
        {
            IMemoryPackFormatter<CookieCollection?> f = CookieFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}

/// <summary>
/// CookieContainer 格式化程序特性
/// </summary>
public sealed class CookieContainerFormatterAttribute : MemoryPackCustomFormatterAttribute<CookieFormatter, CookieContainer?>
{
    /// <summary>
    /// 获取 <see cref="CookieFormatter.Default"/> 实例
    /// </summary>
    public sealed override CookieFormatter GetFormatter() => CookieFormatter.Default;

    /// <summary>
    /// 可空 <see cref="CookieCollection"/> 格式化程序
    /// </summary>
    public sealed class Formatter : MemoryPackFormatter<CookieContainer?>
    {
        /// <summary>
        /// 默认的 <see cref="Formatter"/> 实例
        /// </summary>
        public static readonly Formatter Default = new();

        /// <summary>
        /// 将可空 <see cref="CookieContainer"/> 对象序列化到 <see cref="MemoryPackWriter{TBufferWriter}"/> 对象
        /// </summary>
        /// <typeparam name="TBufferWriter"></typeparam>
        public sealed override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref CookieContainer? value)
        {
            IMemoryPackFormatter<CookieContainer?> f = CookieFormatter.Default;
            f.Serialize(ref writer, ref value);
        }

        /// <summary>
        /// 从 <see cref="MemoryPackReader"/> 对象反序列化为可空 <see cref="CookieContainer"/> 对象
        /// </summary>
        public sealed override void Deserialize(ref MemoryPackReader reader, scoped ref CookieContainer? value)
        {
            IMemoryPackFormatter<CookieContainer?> f = CookieFormatter.Default;
            f.Deserialize(ref reader, ref value);
        }
    }
}