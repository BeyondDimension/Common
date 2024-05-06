using GenerateRepositories = BD.Common8.SourceGenerator.Repositories.Models.Attributes.GenerateRepositoriesAttribute;

namespace BD.Common8.SourceGenerator.Repositories.Templates.Abstractions;

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

    /// <inheritdoc cref="GenerateRepositories"/>
    GenerateRepositories GenerateRepositoriesAttribute { get; }
}