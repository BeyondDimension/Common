using _ThisAssembly_ = BD.Common.DynamicHttpWindowsProxy.ConsoleTest.ThisAssembly;

[assembly: AssemblyProduct(_ThisAssembly_.AssemblyProduct)]

// ReSharper disable once CheckNamespace
namespace BD.Common.DynamicHttpWindowsProxy.ConsoleTest;

static partial class ThisAssembly
{
    /// <summary>
    /// 定义程序集清单的产品名自定义属性
    /// </summary>
    public const string AssemblyProduct = "次元超越动态 Windows 系统代理测试";
}