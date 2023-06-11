namespace BD.Common.Repositories.SourceGenerator.Templates;

/// <summary>
/// 仓储层接口类型源码生成模板
/// </summary>
sealed class RepositoryTemplate : RepositoryTemplateBase<RepositoryTemplate, RepositoryTemplate.Metadata>
{
    public readonly record struct Metadata(
        string Namespace,
        string Summary,
        string ClassName,
        string? PrimaryKeyTypeName = null) : ITemplateMetadata
    {

    }

    protected override void WriteCore(Stream stream, object?[] args, Metadata metadata, ImmutableArray<PropertyMetadata> fields)
    {
        WriteSourceHeader(stream);
        var format =
"""
namespace {0};

/// <summary>
/// {1} - 仓储层接口
/// </summary>
public partial interface I{2}Repository : IRepository<{2}, {3}>, IEFRepository
"""u8;
        args[0] = string.Format(args[0]!.ToString(), "Repositories.Abstractions");
        args[3] = fields.Single(x => x.FixedProperty == FixedProperty.Id).PropertyType;
        stream.WriteFormat(format, args);

        stream.Write(
"""

{

"""u8);

        #region 生成方法 Methods

        WriteByFields(stream, metadata, fields);

        #endregion

        stream.Write(
"""
}

"""u8);
    }
}
