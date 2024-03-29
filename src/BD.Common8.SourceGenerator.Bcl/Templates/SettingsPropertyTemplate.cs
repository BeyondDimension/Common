using BD.Common8.Settings5.Infrastructure;

namespace BD.Common8.SourceGenerator.Bcl.Templates;

[Generator]
public sealed class SettingsPropertyTemplate :
    GeneratedAttributeTemplateBase<
        SettingsPropertyTemplate.AttributeModel,
        SettingsPropertyTemplate.SourceModel>
{
    protected override string Id =>
        "SettingsProperty";

    protected override string AttrName =>
        "BD.Common8.Settings5.Infrastructure.SettingsPropertyGeneratedAttribute";

    public readonly record struct AttributeModel
    {
        /// <inheritdoc cref="SettingsPropertyGeneratedAttribute"/>
        public required SettingsPropertyGeneratedAttribute Attribute { get; init; }
    }

    protected override AttributeModel GetAttribute(ImmutableArray<AttributeData> attributes)
    {
        var attribute = attributes.FirstOrDefault(x => x.ClassNameEquals(AttrName));
        attribute.ThrowIsNull();

        if (attribute.ConstructorArguments.FirstOrDefault().GetObjectValue()
            is not INamedTypeSymbol modelType)
            throw new ArgumentOutOfRangeException(nameof(modelType));

        SettingsPropertyGeneratedAttribute attr = new(new TypeStringImpl(modelType));
        return new AttributeModel
        {
            Attribute = attr,
        };
    }

    /// <summary>
    /// 从源码中读取并分析生成器所需要的模型
    /// </summary>
    public readonly record struct SourceModel : ISourceModel
    {
        /// <inheritdoc cref="INamedTypeSymbol"/>
        public required INamedTypeSymbol NamedTypeSymbol { get; init; }

        /// <summary>
        /// 命名空间
        /// </summary>
        public required string Namespace { get; init; }

        /// <summary>
        /// 类型名
        /// </summary>
        public required string TypeName { get; init; }

        /// <inheritdoc cref="AttributeModel"/>
        public required AttributeModel AttrModel { get; init; }

        /// <inheritdoc cref="SettingsPropertyGeneratedAttribute"/>
        public SettingsPropertyGeneratedAttribute Attribute => AttrModel.Attribute;

        /// <inheritdoc/>
        AttributeModel ISourceModel.Attribute => AttrModel;
    }

    protected override SourceModel GetSourceModel(GetSourceModelArgs args)
    {
        SourceModel model = new()
        {
            NamedTypeSymbol = args.symbol,
            Namespace = args.@namespace,
            TypeName = args.typeName,
            AttrModel = args.attr,
        };
        return model;
    }

    enum PropertyInterfaceType
    {
        None,

        IDictionary,

        ICollection,
    }

    static PropertyInterfaceType GetPropertyInterfaceType(ImmutableArray<INamedTypeSymbol> interfaces)
    {
        if (interfaces.Any(x => x.Name == "IDictionary"))
            return PropertyInterfaceType.IDictionary;
        if (interfaces.Any(x => x.Name == "ICollection"))
            return PropertyInterfaceType.ICollection;
        return PropertyInterfaceType.None;
    }

    protected override void WriteFile(Stream stream, SourceModel m)
    {
        var modelTypeSymbol = TypeStringImpl.GetTypeSymbol(m.AttrModel.Attribute.ModelType);
        modelTypeSymbol.ThrowIsNull();

        WriteFileHeader(stream);
        stream.Write(
"""
#pragma warning disable IDE0028 // 使用集合初始值设定项
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable CS8631 // 类型不能用作泛型类型或方法中的类型参数。类型参数的为 Null 性与约束类型不匹配。
#pragma warning disable CS8634 // 类型不能用作泛型类型或方法中的类型参数。类型参数的为 Null 性与 “class” 约束不匹配。

"""u8);
        stream.WriteNewLine();
        WriteNamespace(stream, m.Namespace);
        stream.WriteNewLine();
        var modelType = m.Attribute.ModelType?.Name;
        modelType.ThrowIsNull();
        stream.WriteFormat(
"""
static partial class {0}
"""u8, m.TypeName);
        stream.WriteNewLine();
        stream.WriteCurlyBracketLeft(); // {
        stream.WriteNewLine();

        #region Body

        var mProperties = modelTypeSymbol.GetMembers()
           .OfType<IPropertySymbol>().ToImmutableArray(); // 模型的属性
        var mFields = modelTypeSymbol.GetMembers().OfType<IFieldSymbol>(); // 模型的字段
        foreach (var property in mProperties)
        {
            if (property.IsGeneratorProperty())
                continue; // 如果模型类为 record 则会生成该属性，跳过
            if (property.DeclaredAccessibility != Accessibility.Public)
                continue;

            var propertyType = TypeStringImpl.Parse(property.Type);
            var propertyTypeInterfaces = property.Type.Interfaces;
            var propertyInterfaceType = GetPropertyInterfaceType(propertyTypeInterfaces);

            var defaultPropertyName = $"Default{property.Name}";
            if (!mFields.Any(x => x.Name == defaultPropertyName))
            {
                defaultPropertyName = string.Empty;
            }
            else
            {
                defaultPropertyName = $"{modelType}.{defaultPropertyName}";
            }

            switch (propertyInterfaceType)
            {
                case PropertyInterfaceType.None:
                    stream.WriteFormat(
"""
    /// <inheritdoc cref="{0}.{1}"/>
    public static SettingsProperty<{2}, {0}> {1}
"""u8, modelType, property.Name, propertyType);
                    stream.Write(" { get; }"u8);
                    stream.WriteNewLine();
                    stream.WriteFormat(
"""
        = new({0});

"""u8, defaultPropertyName);
                    break;
                case PropertyInterfaceType.IDictionary:
                    stream.WriteFormat(
"""
    /// <inheritdoc cref="{0}.{1}"/>
    public static SettingsDictionaryProperty<{3}, {4}, {2}, {0}> {1}
"""u8, modelType, property.Name, propertyType,
propertyType.DictionaryKey, propertyType.DictionaryValue.TrimEnd('?'));
                    stream.Write(" { get; }"u8);
                    stream.WriteNewLine();
                    stream.WriteFormat(
"""
        = new({0});

"""u8, defaultPropertyName);
                    break;
                case PropertyInterfaceType.ICollection:
                    stream.WriteFormat(
"""
    /// <inheritdoc cref="{0}.{1}"/>
    public static SettingsCollectionProperty<{3}, {2}, {0}> {1}
"""u8, modelType, property.Name, propertyType,
propertyType.GenericT.TrimEnd('?'));
                    stream.Write(" { get; }"u8);
                    stream.WriteNewLine();
                    stream.WriteFormat(
"""
        = new({0});

"""u8, defaultPropertyName);
                    break;
                default:
                    break;
            }

            stream.WriteNewLine();
        }

        #endregion

        stream.WriteNewLine();
        stream.WriteCurlyBracketRight(); // }
        stream.WriteNewLine();
    }
}
