namespace BD.Common.Services;

interface IFilePickerPlatformService : IInternalFilePickerPlatformService
{
    static new IFilePickerPlatformService? Instance => Ioc.Get_Nullable<IFilePickerPlatformService>();

    IOpenFileDialogService OpenFileDialogService { get; }

    ISaveFileDialogService SaveFileDialogService { get; }

    public interface IServiceBase
    {
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

    public interface IOpenFileDialogService : IServiceBase
    {
        static IOpenFileDialogService? Instance => Ioc.Get_Nullable<IOpenFileDialogService>();

        Task<IEnumerable<IFileResult>> PlatformPickAsync(PickOptions? options, bool allowMultiple = false);
    }

    public interface ISaveFileDialogService : IServiceBase
    {
        static ISaveFileDialogService? Instance => Ioc.Get_Nullable<ISaveFileDialogService>();

        Task<SaveFileResult?> PlatformSaveAsync(PickOptions? options);
    }
}