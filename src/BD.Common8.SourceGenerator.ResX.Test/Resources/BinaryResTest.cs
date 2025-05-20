using System.CodeDom.Compiler;

namespace BD.Common8.SourceGenerator.ResX.Test.Resources;

/// <summary>
/// 测试 BinaryResourceAttribute 的二进制资源示例
/// </summary>
[BinaryResource(
"""
[
  {
    "Path": "..\\..\\..\\res\\AMap_adcode_citycode_20210406"
  }
]
""",
"""
    static readonly Lazy<byte[]> all = new(() =>
    {
        var bytes = {AMapAdcodeCitycode20210406}();
        return bytes;
    }, LazyThreadSafetyMode.ExecutionAndPublication);
""")]
static partial class BinaryResTest
{
    public static void Test()
    {
        var a = all.Value;
        Console.WriteLine($"BinaryResTest.Test.bytes_len: {a.Length}");
    }
}
