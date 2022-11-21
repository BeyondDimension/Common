using _ThisAssembly_ = BD.Common.Pinyin_.ChnCharInfo.ThisAssembly;

[assembly: AssemblyProduct(_ThisAssembly_.AssemblyProduct)]

// ReSharper disable once CheckNamespace
namespace BD.Common.Pinyin_.ChnCharInfo;

static partial class ThisAssembly
{
    /// <summary>
    /// 定义程序集清单的产品名自定义属性
    /// </summary>
    public const string AssemblyProduct = "次元超越拼音 ChnCharInfo 实现库";
}