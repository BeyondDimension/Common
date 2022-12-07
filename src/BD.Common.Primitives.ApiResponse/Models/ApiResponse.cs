// ReSharper disable once CheckNamespace
namespace BD.Common.Models;

/// <summary>
/// API 响应模型
/// </summary>
public partial class ApiResponse : IApiResponse
{
    public bool IsSuccess { get; set; }

    public string[] Messages { get; set; } = Array.Empty<string>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ApiResponse(string message) => new()
    {
        Messages = new[] { message },
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ApiResponse(string[] messages) => new()
    {
        Messages = messages,
    };
}

/// <summary>
/// API 响应模型
/// </summary>
/// <typeparam name="T"></typeparam>
public class ApiResponse<T> : ApiResponse, IApiResponse<T>
{
    public T? Data { get; set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ApiResponse<T>(string message) => new()
    {
        Messages = new[] { message },
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

public interface IApiResponse
{
    /// <summary>
    /// 是否成功
    /// </summary>
    bool IsSuccess { get; set; }

    /// <summary>
    /// 错误消息数组，用换行拼接显示
    /// </summary>
    string[] Messages { get; set; }
}

public interface IApiResponse<out T> : IApiResponse
{
    /// <summary>
    /// 主数据
    /// </summary>
    T? Data { get; }
}