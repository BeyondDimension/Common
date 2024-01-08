namespace BD.Common8.Essentials.Models;

/// <summary>
/// 提供获取文件类型信息的方法
/// </summary>
public sealed class EssentialsFilePickerFileType : IFilePickerFileType
{
    /// <summary>
    /// 获取文件类型的集合
    /// </summary>
    public IEnumerable<Item> Values { get; }

    /// <summary>
    /// 接受一个 Item 对象集合作为参数
    /// </summary>
    public EssentialsFilePickerFileType(
        IEnumerable<Item> values)
    {
        Values = values;
    }

    /// <summary>
    /// 文件类型的构造函数，可变参数版本，接受多个 Item 对象作为参数
    /// </summary>
    /// <param name="values"></param>
    public EssentialsFilePickerFileType(
        params Item[] values)
    {
        Values = values;
    }

    /// <inheritdoc cref="EssentialsFilePickerFileType.EssentialsFilePickerFileType(Item[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator EssentialsFilePickerFileType(Item[] values) => new(values);

    /// <inheritdoc/>
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

    /// <summary>
    /// 文件类型的数据结构
    /// </summary>
    public readonly record struct Item
    {
        /// <summary>
        /// 文件类型名称
        /// </summary>
        public Item(string? name)
        {
            Name = name ?? string.Empty;
        }

        /// <summary>
        /// 获取文件名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 文件模式的集合
        /// </summary>
        public IReadOnlyList<string>? Patterns { get; init; }

        /// <summary>
        /// MIME 类型的集合，用于判断文件类型
        /// </summary>
        public IReadOnlyList<string>? MimeTypes { get; init; }

        /// <summary>
        /// Apple 平台下的统一类型标识符的集合
        /// </summary>
        public IReadOnlyList<string>? AppleUniformTypeIdentifiers { get; init; }
    }
}