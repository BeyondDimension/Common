using static BD.Common8.SourceGenerator.ResX.Constants;

namespace BD.Common8.SourceGenerator.ResX.Templates;

/// <summary>
/// Designer.cs 源文件模板
/// </summary>
[Generator]
public sealed class DesignerTemplate :
    GeneratedAttributeTemplateBase<
        ResXGeneratedCodeAttribute,
        DesignerTemplate.SourceModel>
{
    protected override string Id =>
        "ResXGeneratedCode";

    protected override string AttrName =>
        "System.CodeDom.Compiler.ResXGeneratedCodeAttribute";

    protected override string FileId => "Designer";

    protected override ResXGeneratedCodeAttribute GetAttribute(ImmutableArray<AttributeData> attributes)
    {
        var attribute = attributes.FirstOrDefault(x => x.ClassNameEquals(AttrName));

        var relativeFilePath = attribute.ThrowIsNull().ConstructorArguments.First().Value!.ToString();

        return new(relativeFilePath);
    }

    /// <summary>
    /// 从源码中读取并分析生成器所需要的模型
    /// </summary>
    public readonly record struct SourceModel : ISourceModel
    {
        /// <inheritdoc cref="INamedTypeSymbol"/>
        public required INamedTypeSymbol NamedTypeSymbol { get; init; }

        /// <inheritdoc cref="ResXGeneratedCodeAttribute"/>
        public required ResXGeneratedCodeAttribute Attribute { get; init; }

        /// <summary>
        /// 源码路径
        /// </summary>
        public required string Path { get; init; }

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
        /// 嵌入资源文件的根名称，没有其扩展名但是包含所有完全限定的命名空间名称。 例如，名为 MyApplication.MyResource.en-US.resources 的资源文件的根名称为 MyApplication.MyResource
        /// </summary>
        public required string? ResourceBaseName { get; init; }

        /// <summary>
        /// 生成的类型是否为 <see langword="public"/>
        /// </summary>
        public required bool IsPublic { get; init; }
    }

    protected override SourceModel GetSourceModel(GetSourceModelArgs args)
    {
        var path = Path.GetFullPath(Path.Combine(
            [
                Path.GetDirectoryName(args.m.SemanticModel.SyntaxTree.FilePath),
                ..
                args.attr.RelativeFilePath.Split('\\')
            ]));

        SourceModel model = new()
        {
            NamedTypeSymbol = args.symbol,
            Attribute = args.attr,
            Path = path,
            Text = null,
            Namespace = args.@namespace,
            TypeName = args.typeName,
            ResourceBaseName = GetDefaultResourceBaseName(path),
            IsPublic = false,
        };
        return model;
    }

    readonly record struct RootDataXmlElement
    {
        public required string Name { get; init; }

        public required string Value { get; init; }

        public required string? Comment { get; init; }

        public void WriteSummary(Stream stream)
        {
            string valueTrim = Value.Trim();
            string[] valueValues = valueTrim.Split("\r\n".ToCharArray(),
                StringSplitOptions.RemoveEmptyEntries);

            string? commentTrim = Comment?.Trim();
            string[]? commentValues = commentTrim?.Split("\r\n".ToCharArray(),
                    StringSplitOptions.RemoveEmptyEntries);

            stream.Write(
"""
    /// <summary>
"""u8);
            stream.WriteNewLine();

            foreach (var item in valueValues)
            {
                stream.WriteFormat(
"""
    /// {0}
"""u8, item.Trim());
                stream.WriteNewLine();
            }

            if (commentValues != null)
            {
                for (int i = 0; i < commentValues.Length; i++)
                {
                    var item = commentValues[i];
                    if (i == 0)
                    {
                        stream.Write(
"""
    /// <para/> // comment
"""u8);
                    }
                    stream.WriteFormat(
    """
    /// {0}
"""u8, item.Trim());
                    stream.WriteNewLine();
                }
            }

            stream.Write(
"""
    /// </summary>
"""u8);
            stream.WriteNewLine();
        }
    }

    /// <summary>
    /// 根据 ResX 文件路径获取所有 data 元素
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static IEnumerable<XElement> GetXmlElementsByResXFilePath(string path)
    {
        var elements = XDocument.Load(path).Root.Elements("data");
        return elements ?? Array.Empty<XElement>();
    }

    /// <summary>
    /// 根据 Xml data 元素反序列化为 <see cref="RootDataXmlElement"/>
    /// </summary>
    /// <param name="elements"></param>
    /// <returns></returns>
    static IEnumerable<RootDataXmlElement> DeserializeResXDataElements(IEnumerable<XElement> elements)
    {
        foreach (var element in elements)
        {
            var name = element.Attribute("name")?.Value;
            if (string.IsNullOrWhiteSpace(name))
                continue;
            var value = element.Element("value")?.Value ?? string.Empty;
            var comment = element.Element("comment")?.Value;
            yield return new()
            {
                Name = name!,
                Value = value,
                Comment = comment,
            };
        }
    }

    protected override void WriteFile(Stream stream, SourceModel m)
    {
        WriteFileHeader(stream);
        stream.WriteNewLine();
        WriteNamespace(stream, m.Namespace);
        stream.WriteNewLine();
        stream.Write(
"""
/// <summary>
///   一个强类型的资源类，用于查找本地化的字符串等。
/// </summary>
// 此类是由 BD.Common8.ResXSourceGenerator 通过源生成器自动生成的。
// 若要添加或移除成员，请编辑 .ResX 文件，然后保存文件以重新运行源生成器
"""u8);
        stream.WriteNewLine();
        stream.WriteFormat(
"""
[global::System.CodeDom.Compiler.GeneratedCodeAttribute("BD.Common8.SourceGenerator.ResX.Templates.DesignerTemplate", "{0}")]
"""u8, FileVersion);
        stream.WriteNewLine();
        stream.Write(
"""
[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
"""u8);
        stream.WriteNewLine();
        if (m.IsPublic)
        {
            stream.WriteFormat(
"""
public static partial class {0}
"""u8, m.TypeName);
        }
        else
        {
            stream.WriteFormat(
"""
static partial class {0}
"""u8, m.TypeName);
        }
        stream.WriteNewLine();
        stream.WriteCurlyBracketLeft(); // {
        stream.WriteNewLine();
        stream.WriteFormat("""
    const string baseName = "{0}";

    static global::System.Reflection.Assembly ResourceAssembly => typeof({1}).Assembly;
"""u8, m.ResourceBaseName, m.TypeName);
        stream.WriteNewLine();
        stream.Write(
"""
    static global::System.Resources.ResourceManager? resourceMan;

    static global::System.Globalization.CultureInfo? resourceCulture;

    /// <summary>
    ///   返回此类使用的缓存的 ResourceManager 实例。
    /// </summary>
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
    public static global::System.Resources.ResourceManager ResourceManager
    {
        get
        {
            if (object.ReferenceEquals(resourceMan, null))
            {
                global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager(baseName, ResourceAssembly);
                resourceMan = temp;
            }
            return resourceMan;
        }
    }

    /// <summary>
    ///   重写当前线程的 CurrentUICulture 属性，对
    ///   使用此强类型资源类的所有资源查找执行重写。
    /// </summary>
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
    public static global::System.Globalization.CultureInfo? Culture
    {
        get => resourceCulture;
        set => resourceCulture = value;
    }
"""u8);
        stream.WriteNewLine();
        stream.WriteNewLine();

        var elements = GetXmlElementsByResXFilePath(m.Path);
        var items = DeserializeResXDataElements(elements);
        foreach (var item in items)
        {
            item.WriteSummary(stream);
            stream.WriteFormat(
"""
    public static string {0} => ResourceManager.GetString("{0}", resourceCulture) ?? "";
"""u8, item.Name);
            stream.WriteNewLine();
            stream.WriteNewLine();
        }

        stream.WriteCurlyBracketRight(); // }
        stream.WriteNewLine();
    }
}
