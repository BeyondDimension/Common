#if DEBUG
namespace BD.Common8.Essentials.Helpers;

[Obsolete("不应需要检查是否支持 Essentials 以及其中的内容", true)]
public static partial class CommonEssentials
{
    /// <summary>
    /// 当前运行环境是否支持 Essentials
    /// </summary>
    public static bool IsSupported { get; internal set; }

    /// <summary>
    /// 是否支持保存文件对话框
    /// </summary>
    public static bool IsSupportedSaveFileDialog { get; internal set; }
}
#endif