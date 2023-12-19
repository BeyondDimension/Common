#if ANDROID
using Android.Content;
using static Android.Content.ClipboardManager;
using Context = Android.Content.Context;
using Application = Android.App.Application;
#elif WINDOWS
using Windows.ApplicationModel.DataTransfer;
using WindowsClipboard = Windows.ApplicationModel.DataTransfer.Clipboard;
#endif

namespace BD.Common8.Essentials.Services.Implementation;

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable CA1822 // 将成员标记为 static

/// <summary>
/// 剪贴板平台服务实现
/// https://github.com/dotnet/maui/tree/8.0.0-rc.2.9373/src/Essentials/src/Clipboard
/// </summary>
sealed class ClipboardPlatformServiceImpl : IClipboardPlatformService
{
    /// <inheritdoc/>
    ValueTask IClipboardPlatformService.PlatformSetTextAsync(string text)
    {
#if ANDROID
        if (ClipboardManager is not null)
            ClipboardManager.PrimaryClip = ClipData.NewPlainText("Text", text ?? string.Empty);
        return default;
#elif WINDOWS
        var dataPackage = new DataPackage();
        dataPackage.SetText(text);
        WindowsClipboard.SetContent(dataPackage);
        return default;
#elif MACOS
        Pasteboard.DeclareTypes(pasteboardTypes, null);
        Pasteboard.ClearContents();
        Pasteboard.SetStringForType(text, pasteboardType);
        return default;
#elif IOS || MACCATALYST
        UIPasteboard.General.String = text;
        return default;
#else
        return default;
#endif
    }

#if WINDOWS
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static async ValueTask<string> WindowsGetTextAsync()
    {
        var clipboardContent = WindowsClipboard.GetContent();
        var hasText = clipboardContent.Contains(StandardDataFormats.Text);
        if (hasText)
        {
            var r = await clipboardContent.GetTextAsync();
            return r ?? string.Empty;
        }
        return string.Empty;
    }
#elif MACOS
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    string? GetPasteboardText()
            => Pasteboard.ReadObjectsForClasses(
                [new ObjCRuntime.Class(typeof(NSString))],
                null)?[0]?.ToString();
#endif

    /// <inheritdoc/>
    ValueTask<string> IClipboardPlatformService.PlatformGetTextAsync()
    {
#if ANDROID
        var r = ClipboardManager?.PrimaryClip?.GetItemAt(0)?.Text;
        return new(r ?? string.Empty);
#elif WINDOWS
        return WindowsGetTextAsync();
#elif MACOS
        var r = GetPasteboardText();
        return new(r ?? string.Empty);
#elif IOS || MACCATALYST
        var r = UIPasteboard.General.String;
        return new(r ?? string.Empty);
#else
        return new(string.Empty);
#endif
    }

    /// <inheritdoc/>
    ValueTask<bool> IClipboardPlatformService.PlatformHasTextAsync()
    {
#if ANDROID
        var r = ClipboardManager is not null &&
            ClipboardManager.HasPrimaryClip &&
            !string.IsNullOrEmpty(ClipboardManager.PrimaryClip?.GetItemAt(0)?.Text);
        return new(r);
#elif WINDOWS
        var r = WindowsClipboard.GetContent().Contains(StandardDataFormats.Text);
        return new(r);
#elif MACOS
        var r = GetPasteboardText();
        return new(!string.IsNullOrEmpty(r));
#elif IOS || MACCATALYST
        var r = UIPasteboard.General.HasStrings;
        return new(r);
#else
        return new(false);
#endif
    }

    event EventHandler<EventArgs>? ClipboardContentChangedInternal;

    /// <inheritdoc/>
    public event EventHandler<EventArgs> ClipboardContentChanged
    {
        add
        {
            if (ClipboardContentChangedInternal == null)
                StartClipboardListeners();
            ClipboardContentChangedInternal += value;
        }

        remove
        {
            ClipboardContentChangedInternal -= value;
            if (ClipboardContentChangedInternal == null)
                StopClipboardListeners();
        }
    }

    /// <summary>
    ///  当剪贴板内容改变时调用的方法，调用剪贴板内容改变的内部事件
    /// </summary>
    /// <param name="sender"></param>
    internal void OnClipboardContentChanged(object? sender = null) =>
        ClipboardContentChangedInternal?.Invoke(sender ?? this, EventArgs.Empty);

    void StartClipboardListeners()
    {
#if ANDROID
        ClipboardManager?.AddPrimaryClipChangedListener(ClipboardListener);
#elif WINDOWS
        WindowsClipboard.ContentChanged += ClipboardChangedEventListener;
#elif IOS || MACCATALYST
        observer = NSNotificationCenter.DefaultCenter.AddObserver(
                UIPasteboard.ChangedNotification,
                ClipboardChangedObserver);
#endif
    }

    void StopClipboardListeners()
    {
#if ANDROID
        ClipboardManager?.RemovePrimaryClipChangedListener(ClipboardListener);
#elif WINDOWS
        WindowsClipboard.ContentChanged -= ClipboardChangedEventListener;
#elif IOS || MACCATALYST
        if (observer is not null)
            NSNotificationCenter.DefaultCenter.RemoveObserver(observer);
#endif
    }

#if ANDROID
    static ClipboardManager? clipboardManager;

    static ClipboardManager? ClipboardManager =>
        clipboardManager ??= Application.Context.GetSystemService(Context.ClipboardService) as ClipboardManager;

    ClipboardChangeListener? clipboardListener;

    ClipboardChangeListener ClipboardListener =>
        clipboardListener ??= new ClipboardChangeListener(this);

    /// <summary>
    /// 初始化 <see cref="ClipboardChangeListener"/> 类的新实例
    /// </summary>
    /// <param name="clipboard">将用于侦听更改的 <see cref="ClipboardPlatformServiceImpl"/> 的实例</param>
    sealed class ClipboardChangeListener(ClipboardPlatformServiceImpl clipboard) : Java.Lang.Object, IOnPrimaryClipChangedListener
    {
        readonly ClipboardPlatformServiceImpl clipboard = clipboard;

        void IOnPrimaryClipChangedListener.OnPrimaryClipChanged() =>
            clipboard.OnClipboardContentChanged();
    }
#elif WINDOWS
    /// <summary>
    /// 触发 <see cref="ClipboardContentChanged"/> 事件的事件侦听器
    /// </summary>
    /// <param name="sender">启动事件的对象</param>
    /// <param name="val">此事件的值</param>
    public void ClipboardChangedEventListener(object? sender, object val)
        => OnClipboardContentChanged(sender);
#elif MACOS
    readonly string pasteboardType = NSPasteboard.NSPasteboardTypeString;
    readonly string[] pasteboardTypes = [NSPasteboard.NSPasteboardTypeString];

    NSPasteboard Pasteboard => NSPasteboard.GeneralPasteboard;
#elif IOS || MACCATALYST
    NSObject? observer;

    /// <summary>
    /// 用于触发 <see cref="ClipboardContentChanged"/> 事件的观察器
    /// </summary>
    /// <param name="notification">触发此事件的通知</param>
    public void ClipboardChangedObserver(NSNotification notification)
        => OnClipboardContentChanged();
#endif
}
