namespace BD.Common.Repositories.SourceGenerator.Templates.Abstractions;

/// <summary>
/// 模板元数据
/// </summary>
public interface ITemplateMetadata
{
    /// <summary>
    /// 命名空间
    /// </summary>
    string Namespace { get; }

    /// <summary>
    /// 类型注释
    /// </summary>
    string Summary { get; }

    /// <summary>
    /// 类型名
    /// </summary>
    string ClassName { get; }
}
