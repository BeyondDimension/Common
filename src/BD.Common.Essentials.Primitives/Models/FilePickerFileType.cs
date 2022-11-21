using System.Collections.ObjectModel;

namespace BD.Common.Models;

public abstract class FilePickerFileType : IFilePickerFileType
{
    IEnumerable<string>? IFilePickerFileType.GetPlatformFileType(Platform platform) => GetPlatformFileType(platform);

    protected abstract IEnumerable<string>? GetPlatformFileType(Platform platform);

    sealed class Fixed : FilePickerFileType
    {
        readonly IEnumerable<string> fileTypes;

        public Fixed(IEnumerable<string> fileTypes)
        {
            this.fileTypes = fileTypes;
        }

        protected override IEnumerable<string>? GetPlatformFileType(Platform _) => fileTypes;
    }

    [return: NotNullIfNotNull("values")]
    public static FilePickerFileType? Parse(IEnumerable<string>? values) => values == null ? null : new Fixed(values);

    [return: NotNullIfNotNull("values")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?(HashSet<string>? values) => Parse(values);

    [return: NotNullIfNotNull("values")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?(string[]? values) => Parse(values);

    [return: NotNullIfNotNull("values")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?(Collection<string>? values) => Parse(values);

    [return: NotNullIfNotNull("values")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?(List<string>? values) => Parse(values);

    sealed class Platform<TValue> : FilePickerFileType where TValue : IEnumerable<string>
    {
        readonly IEnumerable<KeyValuePair<Platform, TValue?>> fileTypes;

        public Platform(IEnumerable<KeyValuePair<Platform, TValue?>> fileTypes)
        {
            this.fileTypes = fileTypes;
        }

        protected override IEnumerable<string>? GetPlatformFileType(Platform platform)
        {
            foreach (var fileType in fileTypes)
            {
                if (fileType.Key.HasFlag(platform))
                    return fileType.Value;
            }
            return null;
        }
    }

    [return: NotNullIfNotNull("values")]
    public static FilePickerFileType? Parse<TValue>(IEnumerable<KeyValuePair<Platform, TValue?>> values) where TValue : IEnumerable<string> => values == null ? null : new Platform<TValue>(values);

    [return: NotNullIfNotNull("values")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?(Dictionary<Platform, IEnumerable<string>?> values) => Parse(values);

    [return: NotNullIfNotNull("values")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?(Dictionary<Platform, string[]?> values) => Parse(values);

    [return: NotNullIfNotNull("values")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?(Dictionary<Platform, List<string>?> values) => Parse(values);

    [return: NotNullIfNotNull("values")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?(Dictionary<Platform, HashSet<string>?> values) => Parse(values);

    [return: NotNullIfNotNull("values")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?(Dictionary<Platform, Collection<string>?> values) => Parse(values);

    public interface IFilePickerFileTypeWithName
    {
        IEnumerable<(string, IEnumerable<string>)> GetFileTypes();
    }

    sealed class Fixed<TValue> : FilePickerFileType, IFilePickerFileTypeWithName where TValue : IEnumerable<string>
    {
        readonly IEnumerable<(string name, TValue extensions)> fileTypes;

        public Fixed(IEnumerable<(string name, TValue extensions)> fileTypes)
        {
            this.fileTypes = fileTypes;
        }

        protected override IEnumerable<string> GetPlatformFileType(Platform _) => fileTypes.SelectMany(x => x.extensions);

        IEnumerable<(string, IEnumerable<string>)> IFilePickerFileTypeWithName.GetFileTypes()
        {
            foreach (var item in fileTypes)
            {
                yield return item;
            }
        }
    }

    [return: NotNullIfNotNull("values")]
    public static FilePickerFileType? Parse<TValue>(IEnumerable<(string name, TValue extensions)> values) where TValue : IEnumerable<string> => values == null ? null : new Fixed<TValue>(values);

    [return: NotNullIfNotNull("values")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?((string name, IEnumerable<string> extensions)[] values) => Parse(values);

    [return: NotNullIfNotNull("values")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?((string name, List<string> extensions)[] values) => Parse(values);

    [return: NotNullIfNotNull("values")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FilePickerFileType?((string name, string[] extensions)[] values) => Parse(values);
}