#if WINDOWS

// ReSharper disable once CheckNamespace
namespace Microsoft.Win32;

public partial interface IRegistryService
{
    static IRegistryService Instance => DefaultRegistryService.Instance.Value;

    /// <inheritdoc cref="Registry2.ReadRegistryKey(string, RegistryView)"/>
    string? ReadRegistryKey(string encodedPath, RegistryView view = Registry2.DefaultRegistryView);

    /// <inheritdoc cref="Registry2.SetRegistryKey(string, RegistryView, string?)"/>
    bool SetRegistryKey(string encodedPath, RegistryView view, string? value = null);

    /// <inheritdoc cref="Registry2.DeleteRegistryKey(string, RegistryView)"/>
    bool DeleteRegistryKey(string encodedPath, RegistryView view = Registry2.DefaultRegistryView);

    /// <summary>
    /// 带参数(可选/null)启动 %windir%\regedit.exe 并等待退出后删除文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="contents"></param>
    /// <param name="millisecondsDelay"></param>
    void StartProcessRegedit(
        string path,
        string contents,
        int millisecondsDelay = 3700);
}

sealed class DefaultRegistryService : IRegistryService
{
    internal static readonly Lazy<IRegistryService> Instance = new(() =>
    {
        var registryService = Ioc.Get_Nullable<IRegistryService>();
        registryService ??= new DefaultRegistryService();
        return registryService;
    });

    bool IRegistryService.DeleteRegistryKey(string encodedPath, RegistryView view)
    {
        return Registry2.DeleteRegistryKey(encodedPath, view);
    }

    string? IRegistryService.ReadRegistryKey(string encodedPath, RegistryView view)
    {
        return Registry2.ReadRegistryKey(encodedPath, view)?.ToString();
    }

    bool IRegistryService.SetRegistryKey(string encodedPath, RegistryView view, string? value)
    {
        return Registry2.SetRegistryKey(encodedPath, view, value);
    }

    void IRegistryService.StartProcessRegedit(string path, string contents, int millisecondsDelay)
    {
        // 仅在 https://github.com/BeyondDimension/SteamTools 上实现
        throw new NotImplementedException();
    }
}

public static partial class RegistryServiceExtensions
{
    #region Registry2

    /// <inheritdoc cref="Registry2.TryReadRegistryKey(string, RegistryView, out object?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadRegistryKey(
        this IRegistryService s,
        string encodedPath,
        RegistryView view,
        [NotNullWhen(true)] out string? value)
    {
        value = s.ReadRegistryKey(encodedPath, view);
        return value != null;
    }

    /// <inheritdoc cref="Registry2.TryReadRegistryKey(string, out object?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadRegistryKey(
        this IRegistryService s,
        string encodedPath,
        [NotNullWhen(true)] out string? value)
    {
        value = s.ReadRegistryKey(encodedPath);
        return value != null;
    }

    /// <inheritdoc cref="Registry2.SetRegistryKey(string, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SetRegistryKey(
        this IRegistryService s,
        string encodedPath,
        string? value = null)
    {
        var result = s.SetRegistryKey(encodedPath, Registry2.DefaultRegistryView, value);
        return result;
    }

    #endregion
}

#endif