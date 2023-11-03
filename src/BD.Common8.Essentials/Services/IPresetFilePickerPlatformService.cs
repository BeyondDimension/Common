namespace BD.Common8.Essentials.Services;

/// <summary>
/// 预设文件选择器平台服务的定义
/// </summary>
public interface IPresetFilePickerPlatformService
{
    /// <summary>
    /// 获取 <see cref="IPresetFilePickerPlatformService"/> 的实例
    /// </summary>
    static IPresetFilePickerPlatformService? Instance => Ioc.Get_Nullable<IPresetFilePickerPlatformService>();

    /// <summary>
    /// 用于选择图像文件的文件类型
    /// </summary>
    IFilePickerFileType Images { get; }

    /// <summary>
    /// 用于选择 PNG 文件的文件类型
    /// </summary>
    IFilePickerFileType Png { get; }

    /// <summary>
    /// 用于选择 JPEG 文件的文件类型
    /// </summary>
    IFilePickerFileType Jpeg { get; }

    /// <summary>
    /// 用于选择视频文件的文件类型
    /// </summary>
    IFilePickerFileType Videos { get; }

    /// <summary>
    /// 用于选择 PDF 文件的文件类型
    /// </summary>
    IFilePickerFileType Pdf { get; }
}
