using BD.Common8.District.Enums;
using BD.Common8.District.Models.Abstractions;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace BD.Common8.District.Models;

/// <inheritdoc cref="IDistrict"/>
[global::MessagePack.MessagePackObject]
[global::MemoryPack.MemoryPackable(global::MemoryPack.SerializeLayout.Explicit)]
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
[BinaryResource(
"""
[
  {
    "Path": "..\\..\\..\\res\\AMap_adcode_citycode_20210406"
  }
]
""",
"""
    static readonly Lazy<District[]> all = new(() =>
    {
        Span<byte> bytes = {AMapAdcodeCitycode20210406}();
        try
        {
            var districts = global::MemoryPack.MemoryPackSerializer.Deserialize<District[]>(bytes);
            return districts!;
        }
        finally
        {
            bytes.Clear();
        }
    }, LazyThreadSafetyMode.ExecutionAndPublication);
""")]
public sealed partial class District : IDistrict
{
    string DebuggerDisplay() => $"{Name}, {Id}";

    /// <inheritdoc/>
    [global::MessagePack.Key(0)]
    [global::MemoryPack.MemoryPackOrder(0)]
    public int Id { get; set; }

    /// <inheritdoc/>
    [global::MessagePack.Key(1)]
    [global::MemoryPack.MemoryPackOrder(1)]
    public string? Name { get; set; }

    /// <inheritdoc/>
    [global::MessagePack.Key(2)]
    [global::MemoryPack.MemoryPackOrder(2)]
    public DistrictLevel Level { get; set; }

    /// <inheritdoc/>
    [global::MessagePack.Key(3)]
    [global::MemoryPack.MemoryPackOrder(3)]
    public int? Up { get; set; }

    /// <inheritdoc/>
    [global::MessagePack.Key(4)]
    [global::MemoryPack.MemoryPackOrder(4)]
    public string? ShortName { get; set; }

    /// <inheritdoc/>
    public override string ToString() => IDistrict.ToString(this);

    /// <summary>
    /// 获取所有行政区域数据
    /// </summary>
    public static District[] All => all.Value;
}
