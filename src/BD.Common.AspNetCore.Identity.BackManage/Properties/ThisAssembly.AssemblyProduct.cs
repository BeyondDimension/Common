using _ThisAssembly_ = BD.Common.AspNetCore.Identity.BackManage.ThisAssembly;

[assembly: AssemblyProduct(_ThisAssembly_.AssemblyProduct)]

// ReSharper disable once CheckNamespace
namespace BD.Common.AspNetCore.Identity.BackManage;

static partial class ThisAssembly
{
    /// <summary>
    /// 定义程序集清单的产品名自定义属性
    /// </summary>
    public const string AssemblyProduct = "次元超越通用多租户后台 Web API 库";
}