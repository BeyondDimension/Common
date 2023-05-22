namespace BD.Common.UI.Helpers;

public static class UIFrameworkHelper
{
    public static void Init(bool isAvaloniaUI = false, bool isMaui = false)
    {
        IsAvaloniaUI = isAvaloniaUI;
        IsMaui = isMaui;
    }

    public static bool IsAvaloniaUI { get; private set; }

    public static bool IsMaui { get; private set; }
}
