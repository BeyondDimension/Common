using _ThisAssembly_ = BD.Common.ThisAssembly;

[assembly: AssemblyProduct(_ThisAssembly_.AssemblyProduct)]
#if WINDOWS7_0_OR_GREATER
[assembly: SupportedOSPlatform("Windows10.0.14393")]
#endif

// ReSharper disable once CheckNamespace
namespace BD.Common;

static partial class ThisAssembly
{
    /// <summary>
    /// 定义程序集清单的产品名自定义属性
    /// </summary>
    public const string AssemblyProduct = "次元超越通用基类库";
}