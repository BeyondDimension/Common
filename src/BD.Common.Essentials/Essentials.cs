namespace BD.Common;

public static partial class CommonEssentials
{
    public static bool IsSupported { get; internal set; }

    /// <summary>
    /// 是否支持保存文件对话框
    /// </summary>
    public static bool IsSupportedSaveFileDialog { get; internal set; }
}
