namespace BD.Common8.Essentials.Models;

#pragma warning disable SA1600 // Elements should be documented

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