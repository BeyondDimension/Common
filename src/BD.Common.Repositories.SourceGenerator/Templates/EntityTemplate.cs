namespace BD.Common.Repositories.SourceGenerator.Templates;

/// <summary>
/// 数据库表实体类型源码生成模板
/// </summary>
sealed class EntityTemplate : TemplateBase<EntityTemplate, EntityTemplate.Metadata>
{
    public readonly record struct Metadata(
        string Namespace,
        string Summary,
        string TableName,
        string ClassName) : ITemplateMetadata
    {

    }

    protected override void WriteCore(Stream stream, object?[] args, Metadata metadata, ImmutableArray<PropertyMetadata> fields)
    {
        var format =
"""
// ReSharper disable once CheckNamespace
namespace {0}.Entities;

/// <summary>
/// {1}
/// </summary>
[Table("{2}")]
public sealed partial class {3}
"""u8;
        stream.WriteFormat(format, args);

        stream.Write(
"""

{

"""u8);

        #region 生成属性 Properties

        for (int i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            field.Write(stream, i, fields.Length);
        }

        #endregion

        stream.Write(
"""
}

"""u8);
    }
}
