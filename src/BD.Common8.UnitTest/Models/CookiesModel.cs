using MessagePack;
using System.Net;
using System.Runtime.Serialization.Formatters;
using SDColor = System.Drawing.Color;

namespace BD.Common8.UnitTest.Models;

[global::MessagePack.MessagePackObject, global::MemoryPack.MemoryPackable(global::MemoryPack.SerializeLayout.Explicit)]
public sealed partial class CookiesModel
{
    [global::MessagePack.Key(0), global::MemoryPack.MemoryPackOrder(0)]
    [MessagePackFormatter(typeof(CookieFormatter))]
    [CookieCollectionFormatter]
    public CookieCollection? Cookies { get; set; }

    [global::MessagePack.Key(1), global::MemoryPack.MemoryPackOrder(1)]
    [MessagePackFormatter(typeof(ColorFormatter))]
    [ColorFormatter]
    public SDColor SDColor { get; set; }

    [global::MessagePack.Key(2), global::MemoryPack.MemoryPackOrder(2)]
    [MessagePackFormatter(typeof(ColorFormatter))]
    [NullableColorFormatter]
    public SDColor? SDColor2 { get; set; }

    [global::MessagePack.Key(3), global::MemoryPack.MemoryPackOrder(3)]
    [MessagePackFormatter(typeof(ColorFormatter))]
    [NullableColorFormatter]
    public SDColor? SDColor3 { get; set; }

    //[global::MessagePack.Key(4), global::MemoryPack.MemoryPackOrder(4)]
    //[MessagePackFormatter(typeof(ColorFormatter))]
    //[SplatColorFormatter]
    //public SplatColor SplatColor { get; set; }

    //[global::MessagePack.Key(5), global::MemoryPack.MemoryPackOrder(5)]
    //[MessagePackFormatter(typeof(ColorFormatter))]
    //[NullableSplatColorFormatter]
    //public SplatColor? SplatColor2 { get; set; }

    //[global::MessagePack.Key(6), global::MemoryPack.MemoryPackOrder(6)]
    //[MessagePackFormatter(typeof(ColorFormatter))]
    //[NullableSplatColorFormatter]
    //public SplatColor? SplatColor3 { get; set; }
}