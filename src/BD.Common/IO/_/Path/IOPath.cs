// ReSharper disable once CheckNamespace
namespace System;

/// <summary>
/// 《文件夹(Directory), 文件(File), 路径/目录(Path)》工具类
/// </summary>
public static partial class IOPath
{
    /// <summary>
    /// 获取资源文件路径
    /// </summary>
    /// <param name="resData">资源数据</param>
    /// <param name="resName">资源名称</param>
    /// <param name="resVer">资源文件版本</param>
    /// <param name="fileEx">资源文件扩展名</param>
    /// <returns></returns>
    public static string GetFileResourcePath(byte[] resData, string resName, int resVer, string fileEx)
    {
        var dirPath = Path.Combine(AppDataDirectory, resName);
        var filePath = Path.Combine(dirPath, $"{resName}@{resVer}{fileEx}");
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
            WriteFile();
        }
        else
        {
            if (!File.Exists(filePath))
            {
                var oldFiles = Directory.GetFiles(dirPath);
                if (oldFiles != null)
                {
                    foreach (var oldFile in oldFiles)
                    {
                        FileTryDelete(oldFile);
                    }
                }
                WriteFile();
            }
        }
        void WriteFile() => File.WriteAllBytes(filePath, resData);
        return filePath;
    }

    /// <summary>
    /// 判断路径是否为文件夹，返回 <see cref="FileInfo"/> 或 <see cref="DirectoryInfo"/>，<see langword="true"/> 为文件夹，<see langword="false"/> 为文件，路径不存在则为 <see langword="null"/>
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileInfo"></param>
    /// <param name="directoryInfo"></param>
    /// <returns></returns>
    public static bool? IsDirectory(string path, [NotNullWhen(false)] out FileInfo? fileInfo, [NotNullWhen(true)] out DirectoryInfo? directoryInfo)
    {
        fileInfo = new(path);
        if (fileInfo.Exists) // 路径为文件
        {
            directoryInfo = null;
            return false;
        }
        fileInfo = null;
        directoryInfo = new(path);
        if (directoryInfo.Exists) // 路径为文件夹
        {
            return true;
        }
        directoryInfo = null;
        return null;
    }

    /// <summary>
    /// 允许文件共享 <see cref="FileShare.Read"/> OR <see cref="FileShare.Write"/> OR <see cref="FileShare.Delete"/>
    /// </summary>
    public const FileShare FileShareReadWriteDelete = FileShare.ReadWrite | FileShare.Delete;

    static FileStream OpenReadCore(string filePath) => new(filePath, FileMode.Open, FileAccess.Read, FileShareReadWriteDelete);

    /// <summary>
    /// 尝试打开文件流，使用 <see cref="FileShareReadWriteDelete"/>，打开失败时将返回 <see langword="null"/>
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="writeLog">是否在失败时纪录日志</param>
    /// <returns></returns>
    public static FileStream? OpenRead(string? filePath, bool writeLog = true)
    {
        if (filePath == null) return null;
        TryOpenRead(filePath, out var stream, out var ex);
        if (writeLog && ex != null)
            Log.Error(nameof(OpenRead), ex, $"OpenRead Error, filePath: {filePath}");
        return stream;
    }

#if !NET35
    static bool TryCall<T>(string? filePath, [NotNullWhen(true)] out T? t, out Exception? ex, Func<string, T> func) where T : class
    {
        ex = null;
        t = null;
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            try
            {
                t = func(filePath);
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
    public static bool TryOpen(string? filePath, FileMode mode, FileAccess access, FileShare share, [NotNullWhen(true)] out FileStream? fileStream, out Exception? ex) => TryCall(filePath, out fileStream, out ex, p => new(p, mode, access, share));

    public static bool TryReadAllBytes(string? filePath, [NotNullWhen(true)] out byte[]? byteArray, out Exception? ex) => TryCall(filePath, out byteArray, out ex, File.ReadAllBytes);

    public static bool TryReadAllText(string? filePath, [NotNullWhen(true)] out string? content, out Exception? ex) => TryCall(filePath, out content, out ex, File.ReadAllText);

    public static bool TryReadAllText(string? filePath, Encoding encoding, [NotNullWhen(true)] out string? content, out Exception? ex) => TryCall(filePath, out content, out ex, p => File.ReadAllText(p, encoding));

    public static bool TryReadAllLines(string? filePath, [NotNullWhen(true)] out string[]? lines, out Exception? ex) => TryCall(filePath, out lines, out ex, File.ReadAllLines);

    public static bool TryReadAllLines(string? filePath, Encoding encoding, [NotNullWhen(true)] out string[]? lines, out Exception? ex) => TryCall(filePath, out lines, out ex, p => File.ReadAllLines(p, encoding));

    public static bool TryReadAllLines(string? filePath, [NotNullWhen(true)] out IEnumerable<string>? lines, out Exception? ex) => TryCall(filePath, out lines, out ex, File.ReadLines);

    public static bool TryReadAllLines(string? filePath, Encoding encoding, [NotNullWhen(true)] out IEnumerable<string>? lines, out Exception? ex) => TryCall(filePath, out lines, out ex, p => File.ReadLines(p, encoding));

    static async Task<(bool success, T? data, Exception? ex)> TryCallAsync<T>(string? filePath, Func<string, CancellationToken, Task<T>> func, CancellationToken cancellationToken) where T : class
    {
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            try
            {
                T data;
#pragma warning disable CA2208 // 正确实例化参数异常
                data = await func(filePath, cancellationToken) ?? throw new ArgumentNullException(nameof(data));
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

    public static Task<(bool success, byte[]? byteArray, Exception? ex)> TryReadAllBytesAsync(string? filePath, CancellationToken cancellationToken = default) => TryCallAsync(filePath, File.ReadAllBytesAsync, cancellationToken);

    public static Task<(bool success, string[]? lines, Exception? ex)> TryReadAllLinesAsync(string? filePath, CancellationToken cancellationToken = default) => TryCallAsync(filePath, File.ReadAllLinesAsync, cancellationToken);

    public static Task<(bool success, string[]? lines, Exception? ex)> TryReadAllLinesAsync(string? filePath, Encoding encoding, CancellationToken cancellationToken = default) => TryCallAsync(filePath, (p, tk) => File.ReadAllLinesAsync(p, encoding, tk), cancellationToken);

    public static Task<(bool success, string? content, Exception? ex)> TryReadAllTextAsync(string? filePath, CancellationToken cancellationToken = default) => TryCallAsync(filePath, File.ReadAllTextAsync, cancellationToken);

    public static Task<(bool success, string? content, Exception? ex)> TryReadAllTextAsync(string? filePath, Encoding encoding, CancellationToken cancellationToken = default) => TryCallAsync(filePath, (p, tk) => File.ReadAllTextAsync(p, encoding, tk), cancellationToken);
#endif
}
