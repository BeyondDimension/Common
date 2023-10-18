namespace BD.Common8.Primitives.ApiRsp.Helpers;

public static partial class ApiRspHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ApiRspImpl<T?> ClientDeserializeFail<T>() => Code<T>(ApiRspCode.ClientDeserializeFail);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Type GetDeserializeType<T>()
    {
        var typeT = typeof(T);
        if (typeT == typeof(object) || typeT.FullName == "System.nil")
            return typeof(ApiRspImpl);
        return typeof(ApiRspImpl<T>);
    }

#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    /// <summary>
    /// 使用 MessagePack 反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<IApiRsp<T?>> DeserializeAsync<T>(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        var type = GetDeserializeType<T>();
        var lz4Options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);
        var obj = await MessagePackSerializer.DeserializeAsync(
            type, stream, options: lz4Options, cancellationToken: cancellationToken);
        if (obj is IApiRsp<T?> rsp) return rsp;
        return ClientDeserializeFail<T>();
    }

    /// <summary>
    /// 使用 MessagePack 反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static IApiRsp<T?> Deserialize<T>(byte[] buffer)
    {
        var type = GetDeserializeType<T>();
        var lz4Options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);
        var obj = MessagePackSerializer.Deserialize(type, buffer, options: lz4Options);
        if (obj is IApiRsp<T?> rsp) return rsp;
        return ClientDeserializeFail<T>();
    }
#endif

#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    /// <summary>
    /// 使用 MemoryPack 反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<IApiRsp<T?>> MemoryPackDeserializeAsync<T>(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var type = GetDeserializeType<T>();
            var obj = await MemoryPackSerializer.DeserializeAsync(
                type, stream, cancellationToken: cancellationToken);
            if (obj is IApiRsp<T?> rsp)
                return rsp;
            return ClientDeserializeFail<T>();
        }
        catch
        {
            var type = typeof(ApiRspImpl);
            var obj = await MemoryPackSerializer.DeserializeAsync(
                type, stream, cancellationToken: cancellationToken);
            if (obj is IApiRsp rsp)
                return Create<T?>(rsp);
            return ClientDeserializeFail<T>();
        }
    }
#endif

#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
#pragma warning disable SA1600 // Elements should be documented
    // https://github.com/dotnet/runtime/blob/v8.0.0-rc.1.23419.4/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/JsonSerializer.Helpers.cs#L13-L14
    internal const string SerializationUnreferencedCodeMessage = "JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.";
    internal const string SerializationRequiresDynamicCodeMessage = "JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.";
#pragma warning restore SA1600 // Elements should be documented

    /// <summary>
    /// 使用 System.Text.Json 反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
    public static async ValueTask<IApiRsp<T?>> JsonPackDeserializeAsync<T>(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        var type = GetDeserializeType<T>();
        var obj = await SystemTextJsonSerializer.DeserializeAsync(
            stream, type, cancellationToken: cancellationToken);
        if (obj is IApiRsp<T?> rsp) return rsp;
        return ClientDeserializeFail<T>();
    }
#endif

    /// <summary>
    /// 尝试从 <see cref="IApiRsp{TContent}"/> 中取出 Content，返回 <see langword="true"/> 时，Content 必定不为 <see langword="null"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetContent<T>(
         this IApiRsp<T> response,
         [NotNullWhen(true)] out T? content)
    {
        content = response.Content;
        return response.IsSuccess && content is not null;
    }

    /// <summary>
    /// 根据状态码，消息，异常创建 <see cref="ApiRspImpl"/> 实例
    /// </summary>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ApiRspImpl Code(ApiRspCode code, string? message = null, Exception? exception = null) => new()
    {
        Code = code,
        InternalMessage = message,
        ClientException = exception,
    };

    static readonly Lazy<ApiRspImpl> okRsp = new(() => Code(ApiRspCode.OK));

    /// <summary>
    /// 获取成功的 <see cref="ApiRspImpl"/> 实例
    /// </summary>
    /// <returns></returns>
    public static ApiRspImpl Ok() => okRsp.Value;

    /// <summary>
    /// 根据状态码，消息，异常创建 <see cref="ApiRspImpl{TContent}"/> 实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <param name="content"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ApiRspImpl<T?> Code<T>(
        ApiRspCode code,
        string? message,
        T? content,
        Exception? exception = null) => new()
        {
            Code = code,
            InternalMessage = message,
            Content = content,
            ClientException = exception,
        };

    /// <summary>
    /// 根据状态码，消息创建 <see cref="ApiRspImpl{TContent}"/> 实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ApiRspImpl<T?> Code<T>(
        ApiRspCode code,
        string? message = null) => Code<T>(code, message, default);

    /// <summary>
    /// 根据内容创建成功的 <see cref="ApiRspImpl{TContent}"/> 实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ApiRspImpl<T?> Ok<T>(T? content = default)
        => Code(ApiRspCode.OK, null, content);

    /// <summary>
    /// 根据消息创建失败的 <see cref="ApiRspImpl{TContent}"/> 实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ApiRspImpl<T?> Fail<T>(string? message = null)
        => Code<T>(ApiRspCode.Fail, message);

    /// <summary>
    /// 根据消息创建失败的 <see cref="ApiRspImpl"/> 实例
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ApiRspImpl Fail(string? message = null)
        => Code(ApiRspCode.Fail, message);

    /// <summary>
    /// 根据异常创建失败的 <see cref="ApiRspImpl"/> 实例
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ApiRspImpl Exception(Exception exception)
        => Code(ApiRspCode.ClientException, null, exception);

    /// <summary>
    /// 根据异常创建失败的 <see cref="ApiRspImpl{TContent}"/> 实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="exception"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ApiRspImpl<T?> Exception<T>(Exception exception)
        => Code<T>(ApiRspCode.ClientException, null, default, exception);

    /// <summary>
    /// 获取内部消息字符串值
    /// </summary>
    /// <param name="rsp"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetInternalMessage(IApiRsp rsp) => rsp is ApiRspBase rspBase ? rspBase.InternalMessage : null;

    /// <summary>
    /// 根据 <see cref="IApiRsp"/> 创建一个新的 <see cref="ApiRspImpl{TContent}"/> 实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rsp"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ApiRspImpl<T?> Create<T>(IApiRsp rsp) =>
        rsp is ApiRspImpl<T?> rspT2 ? rspT2 : Code(rsp.Code, GetInternalMessage(rsp), rsp is IApiRsp<T?> rspT ? rspT.Content : default, rsp is ApiRspBase b ? b.ClientException : default);
}
