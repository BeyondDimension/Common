namespace BD.Common8.SourceGenerator.Bcl.Templates;

/// <summary>
/// 用于标注需要使用 CopyProperties 的源生成器与模板
/// </summary>
[Generator]
public sealed class CopyPropertiesTemplate :
    GeneratedAttributeTemplateBase<
        CopyPropertiesGeneratedAttribute,
        CopyPropertiesTemplate.SourceModel>
{
    protected override string Id =>
        "CopyProperties";

    protected override string AttrName =>
        "System.Runtime.CompilerServices.CopyPropertiesGeneratedAttribute";

    protected override CopyPropertiesGeneratedAttribute GetAttribute(ImmutableArray<AttributeData> attributes)
    {
        return GetMultipleAttributes(attributes).FirstOrDefault();
    }

    protected override IEnumerable<CopyPropertiesGeneratedAttribute>? GetMultipleAttributes(ImmutableArray<AttributeData> attributes)
    {
        var all = attributes.Where(x => x.ClassNameEquals(AttrName));

        List<CopyPropertiesGeneratedAttribute> generatedAttributes = new();

        foreach (var attribute in all)
        {
            CopyPropertiesGeneratedAttribute result = new();

            var destType = attribute.ThrowIsNull().ConstructorArguments.FirstOrDefault();
            if (!destType.IsNull)
            {
                if (destType.Value is ITypeSymbol typeSymbol)
                {
                    result.DestType = new TypeStringImpl(typeSymbol);
                }
            }

            foreach (var item in attribute.ThrowIsNull().NamedArguments)
            {
                var value = item.Value;
                switch (item.Key)
                {
                    case nameof(CopyPropertiesGeneratedAttribute.IgnoreProperties):
                        result.IgnoreProperties = value.GetStrings();
                        break;
                    case nameof(CopyPropertiesGeneratedAttribute.OnlyProperties):
                        result.OnlyProperties = value.GetStrings();
                        break;
                    case nameof(CopyPropertiesGeneratedAttribute.MapProperties):
                        result.MapProperties = value.Value?.ToString();
                        break;
                    case nameof(CopyPropertiesGeneratedAttribute.MethodName):
                        result.MethodName = value.Value?.ToString();
                        break;
                }
            }
            generatedAttributes.Add(result);
        }

        return generatedAttributes;
    }

    /// <summary>
    /// 从源码中读取并分析生成器所需要的模型
    /// </summary>
    public readonly record struct SourceModel : ISourceModel
    {
        /// <inheritdoc cref="INamedTypeSymbol"/>
        public required INamedTypeSymbol Symbol { get; init; }

        /// <summary>
        /// 命名空间
        /// </summary>
        public required string Namespace { get; init; }

        /// <summary>
        /// 类型名
        /// </summary>
        public required string TypeName { get; init; }

        /// <inheritdoc cref="CopyPropertiesGeneratedAttribute"/>
        public required CopyPropertiesGeneratedAttribute Attribute { get; init; }
    }

    protected override SourceModel GetSourceModel(GetSourceModelArgs args)
    {
        SourceModel model = new()
        {
            Symbol = args.symbol,
            Namespace = args.@namespace,
            TypeName = args.typeName,
            Attribute = args.attr,
        };
        return model;
    }

    protected override void WriteFile(Stream stream, SourceModel m)
    {
        var destSymbol = m.Attribute.DestType != null ?
           TypeStringImpl.GetTypeSymbol(m.Attribute.DestType) : m.Symbol;

        var destNamespace = destSymbol!.ContainingNamespace.ToDisplayString();

        WriteFileHeader(stream);
        stream.WriteNewLine();
        WriteNamespace(stream, destNamespace);
        stream.WriteNewLine();

        stream.WriteFormat(
"""
public static partial class {0}
"""u8, $"{destSymbol?.Name}Extensions");
        stream.WriteNewLine();
        stream.WriteCurlyBracketLeft();
        stream.WriteNewLine();

        stream.WriteFormat(
"""
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void {0}(this {1} context, {2} value)
"""u8, m.Attribute.MethodName ?? $"Set{destSymbol?.Name}", destSymbol?.Name, m.TypeName);
        stream.WriteNewLine();
        stream.Write(
"""
    {
"""u8);
        stream.WriteNewLine();
        var cProperties = m.Symbol.GetMembers().OfType<IPropertySymbol>();
        var destProperties = destSymbol!.GetMembers().OfType<IPropertySymbol>();

        foreach (var property in cProperties)
        {
            if (!IsGenerated(m.Attribute, property.Name))
                continue;

            var d_property = destProperties.FirstOrDefault(x => x.Name == property.Name);
            if (d_property == null)
            {
                var mapname = GetMapProperties(m.Attribute, property.Name);
                if (string.IsNullOrEmpty(mapname))
                    continue;
                d_property = destProperties.FirstOrDefault(x => x.Name == mapname);
            }
            if (d_property != null && d_property.Type.ToDisplayString() == property.Type.ToDisplayString())
            {
                stream.WriteFormat(
"""
        context.{0} = value.{1};
"""u8, d_property.Name, property.Name);
                stream.WriteNewLine();
            }
        }
        stream.Write(
"""
    }
"""u8);
        stream.WriteNewLine();
        stream.WriteCurlyBracketRight();
        stream.WriteNewLine();
    }

    /// <summary>
    /// 根据属性名以及 attr 配置项判断是否需要生成属性
    /// </summary>
    /// <param name="attr"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    static bool IsGenerated(CopyPropertiesGeneratedAttribute attr, string propertyName)
    {
        if (attr.IgnoreProperties == null)
        {
            if (attr.OnlyProperties != null)
            {
                return attr.OnlyProperties.Contains(propertyName);
            }
            else
            {
                return true;
            }
        }
        else if (attr.IgnoreProperties.Contains(propertyName))
        {
            return false;
        }
        else
        {
            if (attr.OnlyProperties != null)
            {
                return attr.OnlyProperties.Contains(propertyName);
            }
            return true;
        }
    }

    static string? GetMapProperties(CopyPropertiesGeneratedAttribute attr, string propertyName)
    {
        if (!string.IsNullOrWhiteSpace(attr.MapProperties))
        {
            var mapProperties = JsonConvert.DeserializeObject<Dictionary<string, string>>(attr.MapProperties!);
            if (mapProperties != null && mapProperties.TryGetValue(propertyName, out string value))
                return value;
        }
        return null;
    }
}
