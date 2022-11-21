using _ThisAssembly_ = BD.Common.Pinyin_.TinyPinyin.ThisAssembly;

[assembly: AssemblyProduct(_ThisAssembly_.AssemblyProduct)]

// ReSharper disable once CheckNamespace
namespace BD.Common.Pinyin_.TinyPinyin;

static partial class ThisAssembly
{
    const string ImplLibName =
#if ANDROID
        "TinyPinyin";
#else
        "TinyPinyin.Net";
#endif

    /// <summary>
    /// 定义程序集清单的产品名自定义属性
    /// </summary>
    public const string AssemblyProduct = $"次元超越拼音 {ImplLibName} 实现库";
}