using Splat;
using SDColor = System.Drawing.Color;

namespace BD.Common.UnitTest.Models;

[MPObj, MP2Obj(SerializeLayout.Explicit)]
public sealed partial class CookiesModel
{
    [MPKey(0), MP2Key(0)]
    [MessagePackFormatter(typeof(CookieCollectionFormatter))]
    [CookieCollectionFormatter]
    public CookieCollection? Cookies { get; set; }

    [MPKey(1), MP2Key(1)]
    [MessagePackFormatter(typeof(ColorFormatter2))]
    [ColorFormatter]
    public SDColor SDColor { get; set; }

    [MPKey(2), MP2Key(2)]
    [MessagePackFormatter(typeof(SplatColorFormatter))]
    [SplatColorFormatter]
    public SplatColor SplatColor { get; set; }

    [MPKey(3), MP2Key(3)]
    [MessagePackFormatter(typeof(NullableColorFormatter))]
    [NullableColorFormatter]
    public SDColor? SDColor2 { get; set; }

    [MPKey(4), MP2Key(4)]
    [MessagePackFormatter(typeof(NullableSplatColorFormatter))]
    [NullableSplatColorFormatter]
    public SplatColor? SplatColor2 { get; set; }

    [MPKey(5), MP2Key(5)]
    [MessagePackFormatter(typeof(NullableColorFormatter))]
    [NullableColorFormatter]
    public SDColor? SDColor3 { get; set; }

    [MPKey(6), MP2Key(6)]
    [MessagePackFormatter(typeof(NullableSplatColorFormatter))]
    [NullableSplatColorFormatter]
    public SplatColor? SplatColor3 { get; set; }
}