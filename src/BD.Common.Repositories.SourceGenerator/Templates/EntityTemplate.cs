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
        string ClassName,
        GenerateRepositoriesAttribute GenerateRepositoriesAttribute) : ITemplateMetadata
    {
        /// <inheritdoc cref="GenerateRepositoriesAttribute.NEWSEQUENTIALID"/>
        public bool NEWSEQUENTIALID => GenerateRepositoriesAttribute.NEWSEQUENTIALID;
    }

    public static string GetTableName(INamedTypeSymbol symbol)
    {
        var tableClassName = symbol.GetAttributes().FirstOrDefault(static x => x.ClassNameEquals(TypeFullNames.Table))?.
            ConstructorArguments.FirstOrDefault().Value?.ToString();
        if (string.IsNullOrWhiteSpace(tableClassName))
        {
            tableClassName = symbol.Name;
            tableClassName = GeneratorConfig.Translate(tableClassName);
            tableClassName = tableClassName.Pluralize();
        }
        return tableClassName!;
    }

    protected override void WriteCore(Stream stream, object?[] args, Metadata metadata, ImmutableArray<PropertyMetadata> fields)
    {
        const ClassType classType = ClassType.Entities;
        WriteSourceHeader(stream);
        var format =
"""
namespace {0};

/// <summary>
/// {1} - 表实体模型
/// </summary>
[Table("{2}")]
public sealed partial class {3}
"""u8;
        args[0] = string.Format(args[0]!.ToString(), "Entities");
        stream.WriteFormat(format, args);

        var idField = fields.FirstOrDefault(x => x.FixedProperty == FixedProperty.Id);

        bool isFirstWriteBaseInterfaceType = true;
        var fixedProperties = new HashSet<FixedProperty>(
            fields.Where(x => x.FixedProperty != default)
                .Select(x => x.FixedProperty));
        var baseClassType = fixedProperties.GetEntityBaseClassType();
        if (baseClassType != default)
        {
            var baseType = idField.GetBaseEntityType(baseClassType);
            PropertyMetadata.WriteBaseType(stream, baseType, ref isFirstWriteBaseInterfaceType, false);
        }

        foreach (var field in fields)
        {
            if (field.FixedProperty != default && baseClassType.IsBaseProperty(field.FixedProperty))
                continue; // 父类继承的接口跳过
            field.WriteBaseInterfaceType(stream, classType, ref isFirstWriteBaseInterfaceType);
        }

        if (idField.PropertyType == "Guid")
        {
            if (metadata.NEWSEQUENTIALID)
            {
                PropertyMetadata.WriteBaseType(stream,
                    "INEWSEQUENTIALID"u8, ref isFirstWriteBaseInterfaceType, true);
            }
        }

        stream.Write(
"""

{

"""u8);

        #region 生成属性 Properties

        fields = fields.Where(x => x.FixedProperty == FixedProperty.Id ||
            !baseClassType.IsBaseProperty(x.FixedProperty)).ToImmutableArray();
        for (int i = 0; i < fields.Length; i++)
        {
            bool @override = false;
            var field = fields[i];
            switch (field.FixedProperty)
            {
                case FixedProperty.Id:
                    if (baseClassType.IsBaseProperty(FixedProperty.Id))
                        @override = true;
                    break;
            }
            field.Write(stream, classType, i, fields.Length, @override);
        }

        #endregion

        stream.Write(
"""
}

"""u8);
    }
}
