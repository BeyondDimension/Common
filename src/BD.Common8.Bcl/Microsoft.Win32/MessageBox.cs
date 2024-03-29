// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// https://github.com/dotnet/wpf/blob/v6.0.6/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/MessageBox.cs
// MessageBox 在 NETFRAMEWORK 中直接引用 Wpf 的 PresentationFramework 库而不使用下面的代码

namespace Microsoft.Win32;

#if !NETFRAMEWORK && WINDOWS

public static partial class MessageBox
{
    const uint DEFAULT_BUTTON1 = 0x00000000;
    const uint DEFAULT_BUTTON2 = 0x00000100;
    const uint DEFAULT_BUTTON3 = 0x00000200;

    sealed partial class User32
    {
        /// <summary>
        /// 获取当前活动窗口的句柄
        /// </summary>
        [LibraryImport(nameof(User32), SetLastError = true)]
        public static partial IntPtr GetActiveWindow();

        /// <summary>
        /// 显示带有指定文本、标题和按钮的消息框
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="text">消息框中显示的文本</param>
        /// <param name="caption">消息框的标题</param>
        /// <param name="options">指定要显示的按钮的选项</param>
        [LibraryImport(nameof(User32), EntryPoint = "MessageBoxW", StringMarshalling = StringMarshalling.Utf16)]
        public static partial MessageBoxResult MessageBox(IntPtr hWnd, string text, string caption, uint options);
    }

    /// <summary>
    /// 显示带有指定文本、标题、按钮、图标和行为的消息框，并且指定默认的用户响应
    /// </summary>
    /// <param name="messageBoxText">指定要在消息框中显示的文本</param>
    /// <param name="caption">指定消息框的标题</param>
    /// <param name="button">指定要在消息框中显示的按钮的类型</param>
    /// <param name="icon">指定要在消息框中显示的图标的类型</param>
    /// <param name="defaultResult">指定默认的用户响应</param>
    /// <param name="options">指定其他行为选项</param>
    /// <returns>用户对消息框的响应</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MessageBoxResult Show(
            string messageBoxText,
            string caption,
            MessageBoxButton button,
            MessageBoxImage icon,
            MessageBoxResult defaultResult,
            MessageBoxOptions options) => ShowCore(IntPtr.Zero, messageBoxText, caption, button, icon, defaultResult, options);

    /// <summary>
    /// 显示带有指定文本、标题、按钮、图标的消息框，并且指定默认的用户响应结果
    /// </summary>
    /// <param name="messageBoxText">指定要在消息框中显示的文本</param>
    /// <param name="caption">指定消息框的标题</param>
    /// <param name="button">指定要在消息框中显示的按钮的类型</param>
    /// <param name="icon">指定要在消息框中显示的图标的类型</param>
    /// <param name="defaultResult">指定默认的用户响应</param>
    /// <returns>用户对消息框的响应</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MessageBoxResult Show(
          string messageBoxText,
          string caption,
          MessageBoxButton button,
          MessageBoxImage icon,
          MessageBoxResult defaultResult) => ShowCore(IntPtr.Zero, messageBoxText, caption, button, icon, defaultResult, 0);

    /// <summary>
    /// 显示带有指定文本、标题、按钮、图标的消息框
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MessageBoxResult Show(
         string messageBoxText,
         string caption,
         MessageBoxButton button,
         MessageBoxImage icon) => ShowCore(IntPtr.Zero, messageBoxText, caption, button, icon, 0, 0);

    /// <summary>
    /// 显示带有指定文本、标题、按钮的消息框
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MessageBoxResult Show(
          string messageBoxText,
          string caption,
          MessageBoxButton button) => ShowCore(IntPtr.Zero, messageBoxText, caption, button, MessageBoxImage.None, 0, 0);

    /// <summary>
    /// 显示带有指定文本、标题的消息框
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MessageBoxResult Show(string messageBoxText, string caption) => ShowCore(IntPtr.Zero, messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.None, 0, 0);

    /// <summary>
    /// 显示带有指定文本消息框
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MessageBoxResult Show(string messageBoxText) => ShowCore(IntPtr.Zero, messageBoxText, string.Empty, MessageBoxButton.OK, MessageBoxImage.None, 0, 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static uint DefaultResultToButtonNumber(MessageBoxResult result, MessageBoxButton button)
    {
        if (result == 0) return DEFAULT_BUTTON1;

        switch (button)
        {
            case MessageBoxButton.OK:
                return DEFAULT_BUTTON1;
            case MessageBoxButton.OKCancel:
                if (result == MessageBoxResult.Cancel) return DEFAULT_BUTTON2;
                return DEFAULT_BUTTON1;
            case MessageBoxButton.YesNo:
                if (result == MessageBoxResult.No) return DEFAULT_BUTTON2;
                return DEFAULT_BUTTON1;
            case MessageBoxButton.YesNoCancel:
                if (result == MessageBoxResult.No) return DEFAULT_BUTTON2;
                if (result == MessageBoxResult.Cancel) return DEFAULT_BUTTON3;
                return DEFAULT_BUTTON1;
            default:
                return DEFAULT_BUTTON1;
        }
    }

    /// <summary>
    /// 显示消息框的核心方法
    /// </summary>
    /// <param name="owner">消息框的拥有者窗口句柄</param>
    /// <param name="messageBoxText">要显示的消息文本</param>
    /// <param name="caption">消息框的标题</param>
    /// <param name="button">消息框的按钮类型</param>
    /// <param name="icon">消息框的图标类型</param>
    /// <param name="defaultResult">消息框的结果</param>
    /// <param name="options">消息框的附加选项</param>
    /// <returns>用户在消息框上的操作结果</returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    /// <exception cref="ArgumentException"></exception>
    internal static MessageBoxResult ShowCore(
           IntPtr owner,
           string messageBoxText,
           string caption,
           MessageBoxButton button,
           MessageBoxImage icon,
           MessageBoxResult defaultResult,
           MessageBoxOptions options)
    {
        if (!IsValidMessageBoxButton(button))
        {
            throw new InvalidEnumArgumentException("button", (int)button, typeof(MessageBoxButton));
        }
        if (!IsValidMessageBoxImage(icon))
        {
            throw new InvalidEnumArgumentException("icon", (int)icon, typeof(MessageBoxImage));
        }
        if (!IsValidMessageBoxResult(defaultResult))
        {
            throw new InvalidEnumArgumentException("defaultResult", (int)defaultResult, typeof(MessageBoxResult));
        }
        if (!IsValidMessageBoxOptions(options))
        {
            throw new InvalidEnumArgumentException("options", (int)options, typeof(MessageBoxOptions));
        }

        // UserInteractive??
        //
        /*if (!SystemInformation.UserInteractive && (options & (MessageBoxOptions.ServiceNotification | MessageBoxOptions.DefaultDesktopOnly)) == 0) {
            throw new InvalidOperationException("UNDONE: SR.GetString(SR.CantShowModalOnNonInteractive)");
        }*/

        if ((options & (MessageBoxOptions.ServiceNotification | MessageBoxOptions.DefaultDesktopOnly)) != 0)
        {
            if (owner != IntPtr.Zero)
            {
                // https://github.com/dotnet/winforms/blob/v7.0.3/src/System.Windows.Forms/src/Resources/SR.resx#L504
                throw new ArgumentException("Showing a service notification message box with an owner window is not a valid operation. Use the Show method that does not take an owner.");
            }
        }
        else
        {
            if (owner == IntPtr.Zero)
            {
                owner = User32.GetActiveWindow();
            }
        }

        uint style = (uint)button | (uint)icon | DefaultResultToButtonNumber(defaultResult, button) | (uint)options;

        // modal dialog notification?
        //
        //Application.BeginModalMessageLoop();
        //MessageBoxResult result = Win32ToMessageBoxResult(SafeNativeMethods.MessageBox(new HandleRef(owner, handle), messageBoxText, caption, style));
        MessageBoxResult result = User32.MessageBox(new HandleRef(null, owner).Handle,
            messageBoxText,
            caption,
            style);
        // modal dialog notification?
        //
        //Application.EndModalMessageLoop();

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsValidMessageBoxButton(MessageBoxButton value) => value == MessageBoxButton.OK
           || value == MessageBoxButton.OKCancel
           || value == MessageBoxButton.YesNo
           || value == MessageBoxButton.YesNoCancel;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsValidMessageBoxImage(MessageBoxImage value) => value == MessageBoxImage.Asterisk
           || value == MessageBoxImage.Error
           || value == MessageBoxImage.Exclamation
           || value == MessageBoxImage.Hand
           || value == MessageBoxImage.Information
           || value == MessageBoxImage.None
           || value == MessageBoxImage.Question
           || value == MessageBoxImage.Stop
           || value == MessageBoxImage.Warning;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsValidMessageBoxResult(MessageBoxResult value) => value == MessageBoxResult.Cancel
           || value == MessageBoxResult.No
           || value == MessageBoxResult.None
           || value == MessageBoxResult.OK
           || value == MessageBoxResult.Yes;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsValidMessageBoxOptions(MessageBoxOptions value)
    {
        int mask = ~((int)MessageBoxOptions.ServiceNotification |
                     (int)MessageBoxOptions.DefaultDesktopOnly |
                     (int)MessageBoxOptions.RightAlign |
                     (int)MessageBoxOptions.RtlReading);

        if (((int)value & mask) == 0)
            return true;
        return false;
    }
}

/// <summary>
/// 消息框结果
/// </summary>
public enum MessageBoxResult : byte
{
    None = 0,

    OK = 1,

    Cancel = 2,

    Yes = 6,

    No = 7,

    // NOTE: if you add or remove any values in this enum, be sure to update MessageBox.IsValidMessageBoxResult()
}

/// <summary>
/// 消息框显示方式和行为
/// </summary>
[Flags]
public enum MessageBoxOptions : uint
{
    None = 0x00000000,

    ServiceNotification = 0x00200000,

    DefaultDesktopOnly = 0x00020000,

    RightAlign = 0x00080000,

    RtlReading = 0x00100000,
}

/// <summary>
/// 消息框图标
/// </summary>
public enum MessageBoxImage : uint
{
    None = 0,

    Hand = 0x00000010,

    Question = 0x00000020,

    Exclamation = 0x00000030,

    Asterisk = 0x00000040,

    Stop = Hand,

    Error = Hand,

    Warning = Exclamation,

    Information = Asterisk,

    // NOTE: if you add or remove any values in this enum, be sure to update MessageBox.IsValidMessageBoxIcon()
}

/// <summary>
/// 消息框按钮
/// </summary>
public enum MessageBoxButton : uint
{
    OK = 0x00000000,

    OKCancel = 0x00000001,

    YesNoCancel = 0x00000003,

    YesNo = 0x00000004,

    // NOTE: if you add or remove any values in this enum, be sure to update MessageBox.IsValidMessageBoxButton()
}

#endif