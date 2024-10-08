namespace System;

public static partial class IOPath
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static FileStream OpenReadCore(string filePath) => new(filePath, FileMode.Open, FileAccess.Read, FileShareReadWriteDelete);

    /// <summary>
    /// 尝试打开文件流，使用 <see cref="FileShareReadWriteDelete"/>，打开失败时将返回 <see langword="null"/>
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="writeLog">是否在失败时纪录日志</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FileStream? OpenRead(string? filePath, bool writeLog = true)
    {
        if (filePath == null) return null;
        TryOpenRead(filePath, out var stream, out var ex);
        if (writeLog && ex != null)
            Log.Error(nameof(OpenRead), ex, $"OpenRead Error, filePath: {filePath}");
        return stream;
    }

#if !NET35
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool TryCall<T>(string? filePath, [NotNullWhen(true)] out T? t, out Exception? ex, Func<string, T> func) where T : class
    {
        ex = null;
        t = null;
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            try
            {
                t =
#if NETFRAMEWORK
                    func(filePath!);
#else
                    func(filePath);
#endif
                return true;
            }
            catch (Exception e)
            {
                ex = e;
            }
        }
        return false;
    }

    /// <summary>
    /// 尝试打开文件流，使用 <see cref="FileShareReadWriteDelete"/>
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="fileStream"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryOpenRead(string? filePath, [NotNullWhen(true)] out FileStream? fileStream, out Exception? ex) => TryCall(filePath, out fileStream, out ex, OpenReadCore);

    /// <summary>
    /// 尝试打开文件流
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="mode"></param>
    /// <param name="access"></param>
    /// <param name="share"></param>
    /// <param name="fileStream"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryOpen(string? filePath, FileMode mode, FileAccess access, FileShare share, [NotNullWhen(true)] out FileStream? fileStream, out Exception? ex) => TryCall(filePath, out fileStream, out ex, p => new(p, mode, access, share));

    /// <summary>
    /// 尝试读取所有字节
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="byteArray"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadAllBytes(string? filePath, [NotNullWhen(true)] out byte[]? byteArray, out Exception? ex) => TryCall(filePath, out byteArray, out ex, File.ReadAllBytes);

    /// <summary>
    /// 尝试读取所有文本
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="content"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadAllText(string? filePath, [NotNullWhen(true)] out string? content, out Exception? ex) => TryCall(filePath, out content, out ex, File.ReadAllText);

    /// <summary>
    /// 尝试读取所有文本
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="encoding"></param>
    /// <param name="content"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadAllText(string? filePath, Encoding encoding, [NotNullWhen(true)] out string? content, out Exception? ex) => TryCall(filePath, out content, out ex, p => File.ReadAllText(p, encoding));

    /// <summary>
    /// 尝试读取所有行
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="lines"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadAllLines(string? filePath, [NotNullWhen(true)] out string[]? lines, out Exception? ex) => TryCall(filePath, out lines, out ex, File.ReadAllLines);

    /// <summary>
    /// 尝试读取所有行
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="encoding"></param>
    /// <param name="lines"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadAllLines(string? filePath, Encoding encoding, [NotNullWhen(true)] out string[]? lines, out Exception? ex) => TryCall(filePath, out lines, out ex, p => File.ReadAllLines(p, encoding));

    /// <summary>
    /// 尝试读取所有行
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="lines"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadAllLines(string? filePath, [NotNullWhen(true)] out IEnumerable<string>? lines, out Exception? ex) => TryCall(filePath, out lines, out ex, File.ReadLines);

    /// <summary>
    /// 尝试读取所有行
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="encoding"></param>
    /// <param name="lines"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadAllLines(string? filePath, Encoding encoding, [NotNullWhen(true)] out IEnumerable<string>? lines, out Exception? ex) => TryCall(filePath, out lines, out ex, p => File.ReadLines(p, encoding));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static async Task<(bool success, T? data, Exception? ex)> TryCallAsync<T>(string? filePath, Func<string, CancellationToken, Task<T>> func, CancellationToken cancellationToken) where T : class
    {
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            try
            {
                T data;
#pragma warning disable CA2208 // 正确实例化参数异常
                data =
#if NETFRAMEWORK
                await func(filePath!, cancellationToken)
#else
                await func(filePath, cancellationToken)
#endif
                    ?? throw new ArgumentNullException(nameof(data));
#pragma warning restore CA2208 // 正确实例化参数异常
                return (true, data, null);
            }
            catch (Exception e)
            {
                return (false, null, e);
            }
        }
        return (false, null, null);
    }

#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER

    /// <summary>
    /// 尝试读取所有字节
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<(bool success, byte[]? byteArray, Exception? ex)> TryReadAllBytesAsync(string? filePath, CancellationToken cancellationToken = default) => TryCallAsync(filePath, File.ReadAllBytesAsync, cancellationToken);

    /// <summary>
    /// 尝试读取所有行
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<(bool success, string[]? lines, Exception? ex)> TryReadAllLinesAsync(string? filePath, CancellationToken cancellationToken = default) => TryCallAsync(filePath, File.ReadAllLinesAsync, cancellationToken);

    /// <summary>
    /// 尝试读取所有行
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="encoding"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<(bool success, string[]? lines, Exception? ex)> TryReadAllLinesAsync(string? filePath, Encoding encoding, CancellationToken cancellationToken = default) => TryCallAsync(filePath, (p, tk) => File.ReadAllLinesAsync(p, encoding, tk), cancellationToken);

    /// <summary>
    /// 尝试读取所有行
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<(bool success, string? content, Exception? ex)> TryReadAllTextAsync(string? filePath, CancellationToken cancellationToken = default) => TryCallAsync(filePath, File.ReadAllTextAsync, cancellationToken);

    /// <summary>
    /// 尝试读取所有行
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="encoding"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<(bool success, string? content, Exception? ex)> TryReadAllTextAsync(string? filePath, Encoding encoding, CancellationToken cancellationToken = default) => TryCallAsync(filePath, (p, tk) => File.ReadAllTextAsync(p, encoding, tk), cancellationToken);

#endif

#endif
}