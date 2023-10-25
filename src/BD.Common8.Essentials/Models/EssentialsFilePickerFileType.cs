#pragma warning disable SA1600 // Elements should be documented

namespace BD.Common8.Essentials.Models;

public sealed class EssentialsFilePickerFileType : IFilePickerFileType
{
    public IEnumerable<Item> Values { get; }

    public EssentialsFilePickerFileType(
        IEnumerable<Item> values)
    {
        Values = values;
    }

    public EssentialsFilePickerFileType(
        params Item[] values)
    {
        Values = values;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator EssentialsFilePickerFileType(Item[] values) => new(values);

    IEnumerable<string>? IFilePickerFileType.GetPlatformFileType(DevicePlatform2 platform)
    {
        foreach (var value in Values)
        {
            if (value.Patterns != null)
                foreach (var item in value.Patterns)
                    yield return item;
            if (value.MimeTypes != null)
                foreach (var item in value.MimeTypes)
                    yield return item;
            if (value.AppleUniformTypeIdentifiers != null)
                foreach (var item in value.AppleUniformTypeIdentifiers)
                    yield return item;
        }
    }

    public readonly record struct Item
    {
        public Item(string? name)
        {
            Name = name ?? string.Empty;
        }

        public string Name { get; }

        public IReadOnlyList<string>? Patterns { get; init; }

        public IReadOnlyList<string>? MimeTypes { get; init; }

        public IReadOnlyList<string>? AppleUniformTypeIdentifiers { get; init; }
    }
}