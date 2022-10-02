// ReSharper disable once CheckNamespace
namespace BD.Common.Columns;

public interface IDisable
{
    /// <summary>
    /// 是否禁用该条
    /// </summary>
    bool Disable { get; set; }
}

public interface IDisableReason
{
    /// <summary>
    /// 禁用原因
    /// </summary>
    string? DisableReason { get; set; }
}