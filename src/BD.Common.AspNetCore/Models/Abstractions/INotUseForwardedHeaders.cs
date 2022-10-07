// ReSharper disable once CheckNamespace
namespace BD.Common.Models.Abstractions;

public partial interface INotUseForwardedHeaders
{
    /// <summary>
    /// 不启用反向代理
    /// </summary>
    bool NotUseForwardedHeaders { get; set; }
}