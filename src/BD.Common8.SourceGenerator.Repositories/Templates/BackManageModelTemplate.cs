namespace BD.Common8.SourceGenerator.Repositories.Templates;

/// <summary>
/// 后台管理模型类型源码生成模板
/// </summary>
sealed class BackManageModelTemplate : TemplateBase<BackManageModelTemplate, BackManageModelTemplate.Metadata>
{
    public readonly record struct Metadata(
        string Namespace,
        string Summary,
        string ClassName,
        GenerateRepositoriesAttribute GenerateRepositoriesAttribute) : ITemplateMetadata
    {
        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageAddModel"/>
        public bool BackManageAddModel => GenerateRepositoriesAttribute.BackManageAddModel && GenerateRepositoriesAttribute.BackManageCanAdd;

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageEditModel"/>
        public bool BackManageEditModel => GenerateRepositoriesAttribute.BackManageEditModel && GenerateRepositoriesAttribute.BackManageCanEdit;

        /// <inheritdoc cref="GenerateRepositoriesAttribute.BackManageTableModel"/>
        public bool BackManageTableModel => GenerateRepositoriesAttribute.BackManageTableModel && GenerateRepositoriesAttribute.BackManageCanTable;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsIgnore(
        FixedProperty fixedProperty,
        ClassType classType)
        => fixedProperty switch
        {
            FixedProperty.Id or
            FixedProperty.CreationTime or
            FixedProperty.UpdateTime or
            FixedProperty.CreateUserId or
            FixedProperty.OperatorUserId => classType != ClassType.BackManageTableModels,
            FixedProperty.SoftDeleted => true,
            _ => false,
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool MatchBackManageField(
        BackManageFieldAttribute attr,
        ClassType classType) => classType switch
        {
            ClassType.BackManageAddModels => attr.Add,
            ClassType.BackManageEditModels => attr.Edit || attr.Detail,
            ClassType.BackManageTableModels => attr.Table,
            _ => false,
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ImmutableArray<PropertyMetadata> Filter(
        ImmutableArray<PropertyMetadata> fields,
        ClassType classType,
        bool isOnlyEdit = false)
        => fields.Where(x => (x.BackManageField != null &&
            (!isOnlyEdit || x.BackManageField.Detail == false) &&
            MatchBackManageField(x.BackManageField, classType) &&
            !IsIgnore(x.FixedProperty, classType)) ||
            (x.FixedProperty == FixedProperty.Id && classType == ClassType.BackManageTableModels)).
        ToImmutableArray();

    void WriteAddModel(Stream stream, object?[] args, Metadata metadata, ImmutableArray<PropertyMetadata> fields)
    {
        const ClassType classType = ClassType.BackManageAddModels;
        var format =
"""
/// <summary>
/// {1} - 后台管理添加模型
/// </summary>
public sealed partial class Add{2}DTO
"""u8;
        stream.WriteFormat(format, args);

        fields = Filter(fields, classType);

        var isFirstWriteBaseInterfaceType = true;
        foreach (var field in fields)
        {
            field.WriteBaseInterfaceType(stream, classType, ref isFirstWriteBaseInterfaceType);
        }

        stream.Write(
"""

{

"""u8);

        #region 生成属性 Properties

        for (int i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            field.Write(stream, classType, i, fields.Length);
        }

        #endregion

        stream.Write(
"""
}

"""u8);
    }

    void WriteEditModel(Stream stream, object?[] args, Metadata metadata, ImmutableArray<PropertyMetadata> fields)
    {
        const ClassType classType = ClassType.BackManageEditModels;
        var format =
"""
/// <summary>
/// {1} - 后台管理编辑模型
/// </summary>
public sealed partial class Edit{2}DTO
"""u8;
        stream.WriteFormat(format, args);

        fields = Filter(fields, classType);

        var isFirstWriteBaseInterfaceType = true;
        foreach (var field in fields)
        {
            field.WriteBaseInterfaceType(stream, classType, ref isFirstWriteBaseInterfaceType);
        }

        stream.Write(
"""

{

"""u8);

        #region 生成属性 Properties

        for (int i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            field.Write(stream, classType, i, fields.Length);
        }

        #endregion

        stream.Write(
"""
}

"""u8);
    }

    void WriteTableModel(Stream stream, object?[] args, Metadata metadata, ImmutableArray<PropertyMetadata> fields)
    {
        const ClassType classType = ClassType.BackManageTableModels;
        var format =
"""
/// <summary>
/// {1} - 后台管理表格查询模型
/// </summary>
public sealed partial class Table{2}DTO
"""u8;
        stream.WriteFormat(format, args);

        fields = Filter(fields, classType);

        var isFirstWriteBaseInterfaceType = true;
        foreach (var field in fields)
        {
            field.WriteBaseInterfaceType(stream, classType, ref isFirstWriteBaseInterfaceType);
        }

        stream.Write(
"""

{

"""u8);

        #region 生成属性 Properties

        for (int i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            field.Write(stream, classType, i, fields.Length);
        }

        #endregion

        stream.Write(
"""
}

"""u8);
    }

    protected override void WriteCore(Stream stream, object?[] args, Metadata metadata, ImmutableArray<PropertyMetadata> fields)
    {
        args[0] = string.Format(args[0]!.ToString()!, "Models");
        WriteSourceHeader(stream);
        var format =
"""
namespace {0};

"""u8;
        stream.WriteFormat(format, args);

        if (metadata.BackManageAddModel)
        {
            stream.WriteNewLine();
            WriteAddModel(stream, args, metadata, fields);
        }

        if (metadata.BackManageEditModel)
        {
            stream.WriteNewLine();
            WriteEditModel(stream, args, metadata, fields);
        }

        if (metadata.BackManageTableModel)
        {
            stream.WriteNewLine();
            WriteTableModel(stream, args, metadata, fields);
        }

        // TODO: 参考 VS 插件 TypeScript Definition Generator 2022 生成 typings.d.ts
        // 从 GeneratorConfig.Repositories.json 读取相对路径，例如值为：src\BD.XXX.BackManage.UI
        // 与 GenerateRepositoriesAttribute 中指定的参数合并成要生成文件的路径，例如值为：src\services\XXX
        // 拼接路径为：src\BD.XXX.BackManage.UI\src\services\XXX\typings.d.ts
        // 直接将文件生成在配置指定的目录上，不走源生成只读代码的方案(AddSource)
    }
}
