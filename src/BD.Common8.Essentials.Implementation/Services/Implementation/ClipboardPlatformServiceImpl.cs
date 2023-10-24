#if ANDROID
using Android.Content;
using static Android.Content.ClipboardManager;
using Context = Android.Content.Context;
#elif WINDOWS
using Windows.ApplicationModel.DataTransfer;
using WindowsClipboard = Windows.ApplicationModel.DataTransfer.Clipboard;
#endif

namespace BD.Common8.Essentials.Services.Implementation;

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable CA1822 // 将成员标记为 static

/// <summary>
/// https://github.com/dotnet/maui/tree/8.0.0-rc.2.9373/src/Essentials/src/Clipboard
/// </summary>
sealed class ClipboardPlatformServiceImpl : IClipboardPlatformService
{
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
    /// Initializes a new instance of the <see cref="ClipboardChangeListener"/> class.
    /// </summary>
    /// <param name="clipboard">An instance of <see cref="ClipboardPlatformServiceImpl"/> that will be used to listen for changes.</param>
    sealed class ClipboardChangeListener(ClipboardPlatformServiceImpl clipboard) : Java.Lang.Object, IOnPrimaryClipChangedListener
    {
        readonly ClipboardPlatformServiceImpl clipboard = clipboard;

        void IOnPrimaryClipChangedListener.OnPrimaryClipChanged() =>
            clipboard.OnClipboardContentChanged();
    }
#elif WINDOWS
    /// <summary>
    /// The event listener for triggering the <see cref="ClipboardContentChanged"/> event.
    /// </summary>
    /// <param name="sender">The object that initiated the event.</param>
    /// <param name="val">The value for this event.</param>
    public void ClipboardChangedEventListener(object? sender, object val)
        => OnClipboardContentChanged(sender);
#elif MACOS
    readonly string pasteboardType = NSPasteboard.NSPasteboardTypeString;
    readonly string[] pasteboardTypes = [NSPasteboard.NSPasteboardTypeString];

    NSPasteboard Pasteboard => NSPasteboard.GeneralPasteboard;
#elif IOS || MACCATALYST
    NSObject? observer;

    /// <summary>
    /// The observer for triggering the <see cref="ClipboardContentChanged"/> event.
    /// </summary>
    /// <param name="notification">The notification that triggered this event.</param>
    public void ClipboardChangedObserver(NSNotification notification)
        => OnClipboardContentChanged();
#endif
}
