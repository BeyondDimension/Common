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
        var attribute = attributes.FirstOrDefault(x => x.ClassNameEquals(AttrName));

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
            }
        }

        return result;
    }

    /// <summary>
    /// 从源码中读取并分析生成器所需要的模型
    /// </summary>
    public readonly record struct SourceModel : ISourceModel
    {
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
            Namespace = args.@namespace,
            TypeName = args.typeName,
            Attribute = args.attr,
        };
        return model;
    }

    protected override void WriteFile(Stream stream, SourceModel m)
    {
        WriteFileHeader(stream);
        stream.WriteNewLine();
        WriteNamespace(stream, m.Namespace);
        stream.WriteNewLine();

        // TODO
        // GeneratedAttributeTemplateBase 基类是否支持？多个 attr 即 AllowMultiple = true
        // attr 标注两个类型，可以是同一个类型
        // 循环两个类型的属性，如果属性名相同，且类型相同，生成赋值代码
        // attr 可以白名单指定仅生成的属性名数组，也可黑名单指定不生成的属性名数组
        // 生成扩展函数，表达式数

        //IsGenerated(m.Attribute, "TODO");
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
}
