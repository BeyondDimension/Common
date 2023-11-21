namespace BD.Common8.SourceGenerator.Bcl.Templates;

#pragma warning disable SA1600 // Elements should be documented

sealed class SingletonPartitionTemplate : TemplateBase
{
    const string Id = "SingletonPartition";

    public const string AttrName =
        $"System.Runtime.CompilerServices.{Id}GeneratedAttribute";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static SingletonPartitionGeneratedAttribute GetAttribute(ImmutableArray<AttributeData> attributes)
    {
        var attribute = attributes.FirstOrDefault(static x => x.ClassNameEquals(AttrName));

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

        return result;
    }

    /// <summary>
    /// 从源码中读取并分析生成器所需要的模型
    /// </summary>
    internal readonly record struct SourceModel
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
    }

    public static void Execute(SourceProductionContext spc, GeneratorAttributeSyntaxContext m)
    {
        if (m.TargetSymbol is not INamedTypeSymbol symbol)
            return;

        var @namespace = symbol.ContainingNamespace.ToDisplayString();
        var typeName = symbol.Name;

        var attr = GetAttribute(symbol.GetAttributes());

        SourceModel model = new()
        {
            Namespace = @namespace,
            TypeName = typeName,
            Attribute = attr,
        };
        Execute(spc, model);
    }

    /// <summary>
    /// 源生成器执行逻辑
    /// </summary>
    /// <param name="spc"></param>
    /// <param name="m"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Execute(SourceProductionContext spc, SourceModel m)
    {
        SourceText sourceText;
        try
        {
            using var memoryStream = new MemoryStream();
            WriteFile(memoryStream, m);
            sourceText = SourceText.From(memoryStream, canBeEmbedded: true);
#if DEBUG
            var sourceTextString = sourceText.ToString();
            Console.WriteLine();
            Console.WriteLine(sourceTextString);
#endif
        }
        catch (Exception ex)
        {
            StringBuilder builder = new();
            builder.Append("Namespace: ");
            builder.AppendLine(m.Namespace);
            builder.Append("TypeName: ");
            builder.AppendLine(m.TypeName);
            builder.AppendLine();
            builder.AppendLine(ex.ToString());
            sourceText = builder.ToSourceText();
        }
        spc.AddSource($"{m.Namespace}.{m.TypeName}.{Id}.g.cs", sourceText);
    }

    /// <summary>
    /// 写入源码
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="m"></param>
    public static void WriteFile(
        Stream stream,
        SourceModel m)
    {
        try
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
    static {0}? {1};

    /// <summary>
    /// {3}
    /// </summary>
    public static {0} {2} => {1} ??= new();
"""u8, m.TypeName, GetRandomFieldName(), currentPropertyName, summary);
            stream.WriteNewLine();

            if (m.Attribute.Constructor)
            {
                stream.WriteNewLine();
                stream.WriteFormat(
"""
    {0}() { }
"""u8, m.TypeName);
                stream.WriteNewLine();
            }

            #endregion

            stream.WriteCurlyBracketRight(); // }
            stream.WriteNewLine();
        }
        catch (OperationCanceledException)
        {
        }
    }
}
