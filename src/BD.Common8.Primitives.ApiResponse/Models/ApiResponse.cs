namespace BD.Common8.Primitives.ApiResponse.Models;

/// <summary>
/// <see cref="IApiResponse"/> 的默认实现类
/// </summary>
public partial class ApiResponse : IApiResponse, IApiRsp<object?>
{
    /// <inheritdoc/>
    public bool IsSuccess { get; set; }

    /// <inheritdoc/>
    public string[] Messages { get; set; } = [];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ApiResponse(string message) => new()
    {
        Messages = [message],
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ApiResponse(string[] messages) => new()
    {
        Messages = messages,
    };

    /// <inheritdoc/>
    object? IApiRsp<object?>.Content => null;

    /// <summary>
    /// Ok
    /// </summary>
    public static readonly ApiResponse Ok = new()
    {
        IsSuccess = true,
    };
}

/// <summary>
/// <see cref="IApiResponse{T}"/> 的默认实现类
/// </summary>
/// <typeparam name="T"></typeparam>
public partial class ApiResponse<T> : ApiResponse, IApiResponse<T>
{
    /// <inheritdoc/>
    public T? Data { get; set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ApiResponse<T>(string message) => new()
    {
        Messages = [message],
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ApiResponse<T>(string[] messages) => new()
    {
        Messages = messages,
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ApiResponse<T>(T? data) => new()
    {
        IsSuccess = true,
        Data = data,
    };
}