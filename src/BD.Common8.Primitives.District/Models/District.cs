namespace BD.Common8.Models;

/// <inheritdoc cref="IDistrict"/>
[MPObj]
[MP2Obj(MP2SerializeLayout.Explicit)]
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
[BinaryResource([@"..\..\..\res\AMap_adcode_citycode_20210406"])]
public sealed partial class District : IDistrict
{
    string DebuggerDisplay() => $"{Name}, {Id}";

    /// <inheritdoc/>
    [MPKey(0)]
    [MP2Key(0)]
    public int Id { get; set; }

    /// <inheritdoc/>
    [MPKey(1)]
    [MP2Key(1)]
    public string? Name { get; set; }

    /// <inheritdoc/>
    [MPKey(2)]
    [MP2Key(2)]
    public DistrictLevel Level { get; set; }

    /// <inheritdoc/>
    [MPKey(3)]
    [MP2Key(3)]
    public int? Up { get; set; }

    /// <inheritdoc/>
    [MPKey(4)]
    [MP2Key(4)]
    public string? ShortName { get; set; }

    /// <inheritdoc/>
    public override string ToString() => IDistrict.ToString(this);

    static readonly Lazy<District[]> all = new(() =>
    {
        var districts = MemoryPackSerializer.Deserialize<District[]>(AMapAdcodeCitycode20210406);
        ArgumentNullException.ThrowIfNull(districts);
        return districts;
    }, LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// 获取所有行政区域数据
    /// </summary>
    public static District[] All => all.Value;
}
