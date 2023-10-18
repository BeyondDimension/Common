namespace BD.Common8.Primitives.AdministrativeArea.Models;

/// <summary>
/// 行政区划
/// </summary>
[MPObj]
[MP2Obj(MP2SerializeLayout.Explicit)]
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public sealed partial class AdministrativeArea : IAdministrativeArea
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
    public AdministrativeAreaLevel Level { get; set; }

    /// <inheritdoc/>
    [MPKey(3)]
    [MP2Key(3)]
    public int? Up { get; set; }

    /// <inheritdoc/>
    [MPKey(4)]
    [MP2Key(4)]
    public string? ShortName { get; set; }

    /// <inheritdoc/>
    public override string ToString() => IAdministrativeArea.ToString(this);

    static readonly Lazy<AdministrativeArea[]> values = new(() =>
    {
        ResourceManager resourceManager = new("FxResources.BD.Common8.Primitives.AdministrativeArea.Properties.Resources", typeof(AdministrativeArea).Assembly);
        var areas = MemoryPackSerializer.Deserialize<AdministrativeArea[]>((byte[])resourceManager.GetObject("AMap_adcode_citycode_20210406"));
        ArgumentNullException.ThrowIfNull(areas);
        return areas;
    });

    /// <summary>
    /// 获取所有行政区划数据
    /// </summary>
    public static AdministrativeArea[] Values => values.Value;
}
