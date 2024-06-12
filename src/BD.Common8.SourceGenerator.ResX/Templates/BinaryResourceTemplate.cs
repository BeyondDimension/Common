namespace BD.Common8.SourceGenerator.ResX.Templates;

#pragma warning disable RS1035 // 不要使用禁用于分析器的 API

/// <summary>
/// 用于标注需要根据路径生成函数的二进制嵌入资源
/// </summary>
[Generator]
public sealed class BinaryResourceTemplate :
    GeneratedAttributeTemplateBase<
        BinaryResourceAttribute,
        BinaryResourceTemplate.SourceModel>
{
    protected override string Id =>
        "BinaryResource";

    protected override string AttrName =>
        "System.CodeDom.Compiler.BinaryResourceAttribute";

    protected override BinaryResourceAttribute GetAttribute(ImmutableArray<AttributeData> attributes)
    {
        var attribute = attributes.FirstOrDefault(x => x.ClassNameEquals(AttrName));

        var args = attribute.ThrowIsNull().ConstructorArguments.First().Values.Select(x => x.Value?.ToString()).Where(static x => !string.IsNullOrWhiteSpace(x));

        return new(args.ToArray()!);
    }

    /// <summary>
    /// 从源码中读取并分析生成器所需要的模型
    /// </summary>
    public readonly record struct SourceModel : ISourceModel
    {
        /// <inheritdoc cref="INamedTypeSymbol"/>
        public required INamedTypeSymbol NamedTypeSymbol { get; init; }

        /// <inheritdoc cref="BinaryResourceAttribute"/>
        public required BinaryResourceAttribute Attribute { get; init; }

        /// <summary>
        /// 源码路径
        /// </summary>
        public required string[] FilePaths { get; init; }

        /// <summary>
        /// 源文本
        /// </summary>
        public required SourceText? Text { get; init; }

        /// <summary>
        /// 命名空间
        /// </summary>
        public required string Namespace { get; init; }

        /// <summary>
        /// 类型名
        /// </summary>
        public required string TypeName { get; init; }

        /// <summary>
        /// 生成的类型是否为 <see langword="public"/>
        /// </summary>
        public required bool IsPublic { get; init; }
    }

    protected override SourceModel GetSourceModel(GetSourceModelArgs args)
    {
        if (args.attr.RelativeFilePaths == null || args.attr.RelativeFilePaths.Length == 0)
            return default;

        var queryFilePaths = from x in args.attr.RelativeFilePaths
                             let filePath = Path.GetFullPath(Path.Combine(
                                 [
                                     Path.GetDirectoryName(args.m.SemanticModel.SyntaxTree.FilePath),
                            ..
                            x.Split('\\')
                                 ]))
                             select filePath;

        SourceModel model = new()
        {
            NamedTypeSymbol = args.symbol,
            Attribute = args.attr,
            FilePaths = queryFilePaths.ToArray(),
            Text = null,
            Namespace = args.@namespace,
            TypeName = args.typeName,
            IsPublic = false,
        };
        return model;
    }

    protected override void WriteFile(Stream stream, SourceModel m)
    {
        if (m.FilePaths == null || m.FilePaths.Length == 0)
            return;

        WriteFileHeader(stream);
        stream.WriteNewLine();
        WriteNamespace(stream, m.Namespace);
        stream.WriteNewLine();

        #region partial class
        if (m.IsPublic)
        {
            stream.WriteFormat(
"""
public partial class {0}
"""u8, m.TypeName);
        }
        else
        {
            stream.WriteFormat(
"""
partial class {0}
"""u8, m.TypeName);
        }
        #endregion

        #region {
        stream.WriteNewLine();
        stream.WriteCurlyBracketLeft(); // {
        stream.WriteNewLine();
        #endregion

        foreach (var filePath in m.FilePaths)
        {
            if (!File.Exists(filePath))
                continue;

            var propertyName = Path.GetFileNameWithoutExtension(filePath);
            var propertyNameCharArray = propertyName.ThrowIsNull().ToCharArray();

            stream.Write(
"""
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    static ReadOnlySpan<byte> 
"""u8);
            WriteVariableName(stream, propertyNameCharArray);
            stream.Write(
"""
 => [
"""u8);
            foreach (var b in File.ReadAllBytes(filePath))
            {
                stream.Write("0x"u8);
                stream.WriteUtf16StrToUtf8OrCustom(b.ToString("X"));
                stream.Write(", "u8);
            }
            stream.Write(
"""
];


"""u8);
        }

        #region }
        stream.WriteCurlyBracketRight(); // }
        stream.WriteNewLine();
        #endregion
    }
}