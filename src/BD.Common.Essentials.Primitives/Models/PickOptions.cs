namespace BD.Common.Models;

public sealed class PickOptions
{
    public static PickOptions Default => new()
    {
        FileTypes = null,
    };

    public static PickOptions Images => new()
    {
        FileTypes = IFilePickerFileType.Images(),
    };

    public string? PickerTitle { get; set; }

    public IFilePickerFileType? FileTypes { get; set; }

    public string? InitialFileName { get; set; }
}