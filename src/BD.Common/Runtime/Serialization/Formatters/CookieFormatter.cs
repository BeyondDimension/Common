using MessagePack;
using MessagePack.Formatters;
using KeyAttribute = MessagePack.KeyAttribute;

namespace System.Runtime.Serialization.Formatters;

/// <summary>
/// 对类型 <see cref="Cookie"/>, <see cref="CookieCollection"/>, <see cref="CookieContainer"/> 的序列化与反序列化实现
/// </summary>
public sealed class CookieFormatter : IMessagePackFormatter<Cookie?>, IMessagePackFormatter<CookieCollection?>, IMessagePackFormatter<CookieContainer?>
{
    // https://github.com/neuecc/MessagePack-CSharp/blob/v2.4.59/src/MessagePack.UnityClient/Assets/Scripts/MessagePack/Formatters/CollectionFormatter.cs

    // https://www.coderbusy.com/archives/2002.html

    void IMessagePackFormatter<Cookie?>.Serialize(ref MessagePackWriter writer, Cookie? value, MessagePackSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNil();
        }
        else
        {
            CookieDTO dto = value;
            MessagePackSerializer.Serialize(ref writer, dto, options);
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
            var dto = MessagePackSerializer.Deserialize<CookieDTO>(ref reader, options);
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
        IMessagePackFormatter<CookieCollection?> formatter = this;
        var cookieCollection = formatter.Deserialize(ref reader, options);
        if (cookieCollection != null)
        {
            var container = new CookieContainer();
            container.Add(cookieCollection);
        }
        return default;
    }

    [MessagePackObject]
    public sealed class CookieDTO
    {
        [SerializationConstructor]
        public CookieDTO()
        {

        }

        public CookieDTO(Cookie cookie)
        {
            Comment = cookie.Comment;
            CommentUri = cookie.CommentUri;
            Discard = cookie.Discard;
            Domain = cookie.Domain;
            Expired = cookie.Expired;
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
                Expired = Expired,
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
        public static implicit operator Cookie(CookieDTO cookie)
        => cookie.ToCookie();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator CookieDTO(Cookie cookie)
            => new(cookie);

        /// <inheritdoc cref="Cookie.Comment"/>
        [Key(0)]
        public string Comment { get; set; } = "";

        /// <inheritdoc cref="Cookie.CommentUri"/>
        [Key(1)]
        public Uri? CommentUri { get; set; }

        /// <inheritdoc cref="Cookie.Discard"/>
        [Key(2)]
        public bool Discard { get; set; }

        /// <inheritdoc cref="Cookie.Domain"/>
        [Key(3)]
        public string Domain { get; set; } = null!;

        /// <inheritdoc cref="Cookie.Expired"/>
        [Key(4)]
        public bool Expired { get; set; }

        /// <inheritdoc cref="Cookie.Expires"/>
        [Key(5)]
        public DateTime Expires { get; set; }

        /// <inheritdoc cref="Cookie.HttpOnly"/>
        [Key(6)]
        public bool HttpOnly { get; set; }

        /// <inheritdoc cref="Cookie.Name"/>
        [Key(7)]
        public string Name { get; set; } = null!;

        /// <inheritdoc cref="Cookie.Path"/>
        [Key(8)]
        public string Path { get; set; } = "/";

        /// <inheritdoc cref="Cookie.Port"/>
        [Key(9)]
        public string Port { get; set; } = "";

        /// <inheritdoc cref="Cookie.Secure"/>
        [Key(10)]
        public bool Secure { get; set; }

        /// <inheritdoc cref="Cookie.Value"/>
        [Key(11)]
        public string Value { get; set; } = null!;

        /// <inheritdoc cref="Cookie.Version"/>
        [Key(12)]
        public int Version { get; set; }
    }
}
