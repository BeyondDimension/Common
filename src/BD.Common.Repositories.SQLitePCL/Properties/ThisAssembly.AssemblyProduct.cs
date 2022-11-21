using _ThisAssembly_ = BD.Common.Repositories.SQLitePCL.ThisAssembly;

[assembly: AssemblyProduct(_ThisAssembly_.AssemblyProduct)]

// ReSharper disable once CheckNamespace
namespace BD.Common.Repositories.SQLitePCL;

static partial class ThisAssembly
{
    /// <summary>
    /// 定义程序集清单的产品名自定义属性
    /// </summary>
    public const string AssemblyProduct = "次元超越通用仓储 SQLitePCL 实现库";
}