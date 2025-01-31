namespace BD.Common8.SourceGenerator.Bcl.Templates;

/// <summary>
/// 用于标注需要使用单例(Singleton)模式的服务类源生成器与模板
/// </summary>
[Generator]
public sealed class SingletonPartitionTemplate :
    GeneratedAttributeTemplateBase<
        SingletonPartitionGeneratedAttribute,
        SingletonPartitionTemplate.SourceModel>
{
    protected override string Id =>
        "SingletonPartition";

    protected override string AttrName =>
        "System.Runtime.CompilerServices.SingletonPartitionGeneratedAttribute";

    protected override IEnumerable<SingletonPartitionGeneratedAttribute> GetMultipleAttributes(ImmutableArray<AttributeData> attributes)
    {
        var items = attributes.Where(x => x.ClassNameEquals(AttrName));
        foreach (var attribute in items)
        {
            SingletonPartitionGeneratedAttribute result = new();
            foreach (var item in attribute.ThrowIsNull().NamedArguments)
            {
                var value = item.Value.GetObjectValue();
                switch (item.Key)
                {
                    case nameof(SingletonPartitionGeneratedAttribute.Constructor):
                        result.Constructor = Convert.ToBoolean(value);
                        break;
                    case nameof(SingletonPartitionGeneratedAttribute.Summary):
                        result.Summary = value?.ToString();
                        break;
                    case nameof(SingletonPartitionGeneratedAttribute.PropertyName):
                        result.PropertyName = value?.ToString();
                        break;
                    case nameof(SingletonPartitionGeneratedAttribute.IsSealed):
                        result.IsSealed = Convert.ToBoolean(value);
                        break;
                }
            }
            yield return result;
        }
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

        /// <inheritdoc cref="SingletonPartitionGeneratedAttribute"/>
        public required SingletonPartitionGeneratedAttribute Attribute { get; init; }

        /// <inheritdoc cref="I"/>
        public required int I { get; init; }
    }

    protected override SourceModel GetSourceModel(GetSourceModelArgs args)
    {
        SourceModel model = new()
        {
            I = args.i,
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
        stream.WriteFormat(m.Attribute.IsSealed ?
"""
sealed partial class {0}
"""u8
            :
"""
partial class {0}
"""u8, m.TypeName);
        stream.WriteNewLine();
        stream.WriteCurlyBracketLeft(); // {
        stream.WriteNewLine();

        #region Body

        var currentPropertyName =
            string.IsNullOrWhiteSpace(m.Attribute.PropertyName) ?
            "Current" :
            m.Attribute.PropertyName;
        var summary =
            string.IsNullOrWhiteSpace(m.Attribute.Summary) ?
            "获取当前单例服务" :
            m.Attribute.Summary;
        stream.WriteFormat(
"""
    static readonly Lazy<{0}> {1} = new(() => new(), LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// {3}
    /// </summary>
    public static {0} {2} => {1}.Value;
"""u8, m.TypeName, GetRandomFieldName(Deterministic ? m.TypeName : null), currentPropertyName, summary);
        stream.WriteNewLine();

        if (m.Attribute.Constructor)
        {
            stream.WriteNewLine();
            stream.WriteFormat(
"""
    {0}() { }
"""u8, m.TypeName);
        }

        #endregion

        stream.WriteNewLine();
        stream.WriteCurlyBracketRight(); // }
        stream.WriteNewLine();
    }
}
