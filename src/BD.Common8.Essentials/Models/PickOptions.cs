namespace BD.Common8.Essentials.Models;

/// <summary>
/// 提供文件选择器的选项配置
/// </summary>
public sealed class PickOptions
{
    /// <summary>
    /// 获取默认的选项
    /// </summary>
    public static PickOptions Default => new()
    {
        FileTypes = null,
    };

    /// <summary>
    /// 获取用于选择图像的选项
    /// </summary>
    public static PickOptions Images => new()
    {
        FileTypes = IFilePickerFileType.Images(),
    };

    /// <summary>
    /// 获取或设置选择器的标题
    /// </summary>
    public string? PickerTitle { get; set; }

    /// <summary>
    /// 获取或设置文件类型选项
    /// </summary>
    public IFilePickerFileType? FileTypes { get; set; }

    /// <summary>
    /// 获取或设置初始文件名
    /// </summary>
    public string? InitialFileName { get; set; }
}