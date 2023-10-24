#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace BD.Common8.Essentials.Models;

public abstract class FilePickerFileType : IFilePickerFileType
{
    IEnumerable<string>? IFilePickerFileType.GetPlatformFileType(DevicePlatform2 platform) => GetPlatformFileType(platform);

    protected abstract IEnumerable<string>? GetPlatformFileType(DevicePlatform2 platform);

    sealed class Fixed(IEnumerable<string> fileTypes) : FilePickerFileType
    {
        readonly IEnumerable<string> fileTypes = fileTypes;

        protected override IEnumerable<string>? GetPlatformFileType(DevicePlatform2 _) => fileTypes;
    }

    [return: NotNullIfNotNull(nameof(values))]
    public static FilePickerFileType? Parse(IEnumerable<string>? values) => values == null ? null : new Fixed(values);

    [return: NotNullIfNotNull(nameof(values))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?(HashSet<string>? values) => Parse(values);

    [return: NotNullIfNotNull(nameof(values))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?(string[]? values) => Parse(values);

    [return: NotNullIfNotNull(nameof(values))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?(Collection<string>? values) => Parse(values);

    [return: NotNullIfNotNull(nameof(values))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?(List<string>? values) => Parse(values);

    sealed class Platform<TValue>(IEnumerable<KeyValuePair<DevicePlatform2, TValue?>> fileTypes) : FilePickerFileType where TValue : IEnumerable<string>
    {
        readonly IEnumerable<KeyValuePair<DevicePlatform2, TValue?>> fileTypes = fileTypes;

        protected override IEnumerable<string>? GetPlatformFileType(DevicePlatform2 platform)
        {
            foreach (var fileType in fileTypes)
                if (fileType.Key.HasFlag(platform))
                    return fileType.Value;
            return null;
        }
    }

    [return: NotNullIfNotNull(nameof(values))]
    public static FilePickerFileType? Parse<TValue>(IEnumerable<KeyValuePair<DevicePlatform2, TValue?>> values) where TValue : IEnumerable<string> => values == null ? null : new Platform<TValue>(values);

    [return: NotNullIfNotNull(nameof(values))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?(Dictionary<DevicePlatform2, IEnumerable<string>?> values) => Parse(values);

    [return: NotNullIfNotNull(nameof(values))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?(Dictionary<DevicePlatform2, string[]?> values) => Parse(values);

    [return: NotNullIfNotNull(nameof(values))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?(Dictionary<DevicePlatform2, List<string>?> values) => Parse(values);

    [return: NotNullIfNotNull(nameof(values))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?(Dictionary<DevicePlatform2, HashSet<string>?> values) => Parse(values);

    [return: NotNullIfNotNull(nameof(values))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?(Dictionary<DevicePlatform2, Collection<string>?> values) => Parse(values);

    public interface IFilePickerFileTypeWithName
    {
        IEnumerable<(string name, IEnumerable<string> extensions)> GetFileTypes();
    }

    sealed class Fixed<TValue>(IEnumerable<(string name, TValue extensions)> fileTypes) : FilePickerFileType, IFilePickerFileTypeWithName where TValue : IEnumerable<string>
    {
        readonly IEnumerable<(string name, TValue extensions)> fileTypes = fileTypes;

        protected override IEnumerable<string> GetPlatformFileType(DevicePlatform2 _) => fileTypes.SelectMany(x => x.extensions);

        IEnumerable<(string, IEnumerable<string>)> IFilePickerFileTypeWithName.GetFileTypes()
        {
            foreach (var item in fileTypes)
                yield return item;
        }
    }

    [return: NotNullIfNotNull(nameof(values))]
    public static FilePickerFileType? Parse<TValue>(IEnumerable<(string name, TValue extensions)> values) where TValue : IEnumerable<string> => values == null ? null : new Fixed<TValue>(values);

    [return: NotNullIfNotNull(nameof(values))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?((string name, IEnumerable<string> extensions)[] values) => Parse(values);

    [return: NotNullIfNotNull(nameof(values))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?((string name, List<string> extensions)[] values) => Parse(values);

    [return: NotNullIfNotNull(nameof(values))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?((string name, string[] extensions)[] values) => Parse(values);
}