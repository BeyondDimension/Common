namespace BD.Common8.Essentials.Services;

/// <summary>
/// 用于提供文件选择器的平台服务
/// </summary>
public interface IFilePickerPlatformService : IPresetFilePickerPlatformService
{
    /// <summary>
    /// 获取 <see cref="IFilePickerPlatformService"/> 实例
    /// </summary>
    static new IFilePickerPlatformService? Instance => Ioc.Get_Nullable<IFilePickerPlatformService>();

    /// <summary>
    /// 打开文件对话框服务
    /// </summary>
    IOpenFileDialogService OpenFileDialogService { get; }

    /// <summary>
    /// 保存文件对话框
    /// </summary>
    ISaveFileDialogService SaveFileDialogService { get; }

    /// <summary>
    /// 提供共享的基本服务
    /// </summary>
    public interface IServiceBase
    {
        /// <summary>
        /// 格式化扩展名列表
        /// </summary>
        /// <param name="extensions">扩展名集合</param>
        /// <param name="trimLeadingPeriod">是否删除文件扩展名前面的点号</param>
        /// <returns></returns>
        protected static IEnumerable<string> FormatExtensions(IEnumerable<string>? extensions, bool trimLeadingPeriod = false)
        {
            var hasAtLeastOneType = false;

            if (extensions != null)
            {
                foreach (var extension in extensions)
                {
                    var ext = Clean(extension, trimLeadingPeriod);
                    static string Clean(string extension, bool trimLeadingPeriod = false)
                    {
                        if (string.IsNullOrWhiteSpace(extension))
                            return string.Empty;

                        if (extension.StartsWith('.'))
                        {
                            if (!trimLeadingPeriod) return extension;
                            return extension.TrimStart('.');
                        }
                        else
                        {
                            if (!trimLeadingPeriod) return "." + extension;
                            return extension;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(ext))
                    {
                        yield return ext;
                        hasAtLeastOneType = true;
                    }
                }
            }

            if (!hasAtLeastOneType)
                yield return "*";
        }
    }

    /// <summary>
    /// 提供了打开文件对话框的功能
    /// </summary>
    public interface IOpenFileDialogService : IServiceBase
    {
        /// <summary>
        /// 获取 <see cref="IOpenFileDialogService"/> 实例
        /// </summary>
        static IOpenFileDialogService? Instance => Ioc.Get_Nullable<IOpenFileDialogService>();

        /// <summary>
        /// 打开文件对话框，返回选中的文件结果集合
        /// </summary>
        Task<IEnumerable<IFileResult>> PlatformPickAsync(PickOptions? options, bool allowMultiple = false);
    }

    /// <summary>
    /// 提供了保存文件对话框的功能
    /// </summary>
    public interface ISaveFileDialogService : IServiceBase
    {
        /// <summary>
        /// 获取 <see cref="ISaveFileDialogService"/> 实例
        /// </summary>
        static ISaveFileDialogService? Instance => Ioc.Get_Nullable<ISaveFileDialogService>();

        /// <summary>
        /// 打开保存文件对话框，返回保存的文件结果
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<SaveFileResult?> PlatformSaveAsync(PickOptions? options);
    }
}