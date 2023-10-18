namespace BD.Common8.Primitives.AdministrativeArea.Models;

[MPObj]
[MP2Obj(SerializeLayout.Explicit)]
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public sealed partial class Area : IArea
{
    string DebuggerDisplay() => $"{Name}, {Id}";

    [MPKey(0)]
    [MP2Key(0)]
#if __HAVE_N_JSON__
    [N_JsonProperty("0")]
#endif
#if !__NOT_HAVE_S_JSON__
    [S_JsonProperty("0")]
#endif
    public int Id { get; set; }

    [MPKey(1)]
    [MP2Key(1)]
#if __HAVE_N_JSON__
    [N_JsonProperty("1")]
#endif
#if !__NOT_HAVE_S_JSON__
    [S_JsonProperty("1")]
#endif
    public string? Name { get; set; }

    [MPKey(2)]
    [MP2Key(2)]
#if __HAVE_N_JSON__
    [N_JsonProperty("2")]
#endif
#if !__NOT_HAVE_S_JSON__
    [S_JsonProperty("2")]
#endif
    public AreaLevel Level { get; set; }

    [MPKey(3)]
    [MP2Key(3)]
#if __HAVE_N_JSON__
    [N_JsonProperty("3")]
#endif
#if !__NOT_HAVE_S_JSON__
    [S_JsonProperty("3")]
#endif
    public int? Up { get; set; }

    [MPKey(4)]
    [MP2Key(4)]
#if __HAVE_N_JSON__
    [N_JsonProperty("4")]
#endif
#if !__NOT_HAVE_S_JSON__
    [S_JsonProperty("4")]
#endif
    public string? ShortName { get; set; }

    public override string ToString() => IArea.ToString(this);

    static readonly Lazy<Area[]> values = new(() =>
    {
        //var lz4Options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);
        //var result = MessagePackSerializer.Deserialize<Area[]>(Properties.Resources.AMap_adcode_citycode_20210406_xlsx, lz4Options);
        //return result;
        var areas = MemoryPackSerializer.Deserialize<Area[]>(Properties.Resources.AMap_adcode_citycode_20210406);
        ArgumentNullException.ThrowIfNull(areas);
        return areas;
    });

    public static Area[] Values => values.Value;
}
