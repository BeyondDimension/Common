using BD.Common8.Essentials.Enums;
using BD.Common8.Essentials.Models.Abstractions;
using System.Drawing;
using System.Runtime.Serialization;

namespace BD.Common8.Essentials.Models;

/// <summary>
/// 打开浏览器的可选设置接口
/// </summary>
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
[global::MessagePack.MessagePackObject]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
[global::MemoryPack.MemoryPackable(global::MemoryPack.GenerateType.VersionTolerant, global::MemoryPack.SerializeLayout.Explicit)]
#endif
public sealed partial class BrowserLaunchOptions : IBrowserLaunchOptions
{
    /// <inheritdoc cref="IBrowserLaunchOptions.PreferredToolbarColor"/>
    [IgnoreDataMember]
#if !NO_NEWTONSOFT_JSON
    [global::Newtonsoft.Json.JsonIgnore]
#endif
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [global::System.Text.Json.Serialization.JsonIgnore]
#endif
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [global::MessagePack.IgnoreMember]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [global::MemoryPack.MemoryPackIgnore]
#endif
    public Color? PreferredToolbarColor { get; set; }

    /// <inheritdoc cref="IBrowserLaunchOptions.PreferredToolbarColor"/>
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [global::MessagePack.Key(0)]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [global::MemoryPack.MemoryPackOrder(0)]
#endif
    public int? PreferredToolbarColorInt32
    {
        get => PreferredToolbarColor.HasValue ? PreferredToolbarColor.Value.ToArgb() : null;
        set => PreferredToolbarColor = value.HasValue ? Color.FromArgb(value.Value) : null;
    }

    /// <inheritdoc cref="IBrowserLaunchOptions.PreferredControlColor"/>
    [IgnoreDataMember]
#if !NO_NEWTONSOFT_JSON
    [global::Newtonsoft.Json.JsonIgnore]
#endif
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [global::System.Text.Json.Serialization.JsonIgnore]
#endif
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [global::MessagePack.IgnoreMember]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [global::MemoryPack.MemoryPackIgnore]
#endif
    public Color? PreferredControlColor { get; set; }

    /// <inheritdoc cref="IBrowserLaunchOptions.PreferredControlColor"/>
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [global::MessagePack.Key(1)]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [global::MemoryPack.MemoryPackOrder(1)]
#endif
    public int? PreferredControlColorInt32
    {
        get => PreferredControlColor.HasValue ? PreferredControlColor.Value.ToArgb() : null;
        set => PreferredControlColor = value.HasValue ? Color.FromArgb(value.Value) : null;
    }

    /// <inheritdoc cref="IBrowserLaunchOptions.LaunchMode"/>
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [global::MessagePack.Key(2)]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [global::MemoryPack.MemoryPackOrder(2)]
#endif
    public BrowserLaunchMode LaunchMode { get; set; } = IBrowserLaunchOptions.DefaultLaunchMode;

    /// <inheritdoc cref="IBrowserLaunchOptions.TitleMode"/>
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [global::MessagePack.Key(3)]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [global::MemoryPack.MemoryPackOrder(3)]
#endif
    public BrowserTitleMode TitleMode { get; set; } = BrowserTitleMode.Default;

    /// <inheritdoc cref="IBrowserLaunchOptions.Flags"/>
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [global::MessagePack.Key(4)]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [global::MemoryPack.MemoryPackOrder(4)]
#endif
    public BrowserLaunchFlags Flags { get; set; } = BrowserLaunchFlags.None;
}
