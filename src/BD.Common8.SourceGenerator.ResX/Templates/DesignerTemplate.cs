using static BD.Common8.SourceGenerator.ResX.Constants;

namespace BD.Common8.SourceGenerator.ResX.Templates;

#pragma warning disable RS1035 // 不要使用禁用于分析器的 API

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

    protected override IEnumerable<ResXGeneratedCodeAttribute> GetMultipleAttributes(ImmutableArray<AttributeData> attributes)
    {
        var items = attributes.Where(x => x.ClassNameEquals(AttrName));
        foreach (var attribute in items)
        {
            var relativeFilePath = attribute.ThrowIsNull().ConstructorArguments.First().Value!.ToString();

            if (!(attribute.ConstructorArguments.Length >= 2 && byte.TryParse(attribute.ConstructorArguments[1].Value?.ToString(), out var version)))
            {
                version = 0;
            }

            yield return new(relativeFilePath, version);
        }
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

        /// <summary>
        /// 生成器是否使用 StringResourceManager 的代码模板
        /// </summary>
        public required bool IsSRM { get; init; }

        /// <inheritdoc cref="I"/>
        public required int I { get; init; }
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
            I = args.i,
            NamedTypeSymbol = args.symbol,
            Attribute = args.attr,
            Path = path,
            Namespace = args.@namespace,
            TypeName = args.typeName,
            ResourceBaseName = GetDefaultResourceBaseName(path),
            IsPublic = false,
            IsSRM = args.attr.Version == 1,
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
    /// <para/>
    /// comment: 
"""u8);
                    }
                    var isFirstWriteComment = true;
                    var comments = item.Trim().Split(["\r\n"], StringSplitOptions.RemoveEmptyEntries);
                    foreach (var it in comments)
                    {
                        var comments2 = it.Trim().Split(["\n"], StringSplitOptions.RemoveEmptyEntries);
                        foreach (var it2 in comments2)
                        {
                            if (isFirstWriteComment)
                            {
                                isFirstWriteComment = false;
                            }
                            else
                            {
                                stream.Write(
"""
    /// 
"""u8);
                            }
                            stream.WriteUtf16StrToUtf8OrCustom(it2);
                        }
                    }
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
        return elements ?? [];
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
        if (m.IsSRM)
        {
            stream.Write(
"""
#pragma warning disable RS1035 // 已将该符号标记为禁止在分析器中使用，并应改用备用符号

"""u8);
        }
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
        stream.WriteNewLine();
        stream.WriteCurlyBracketLeft(); // {
        stream.WriteNewLine();

        byte[]? typeNameSRM = m.IsSRM ? Encoding.UTF8.GetBytes(GetRandomClassName(Deterministic ? $"{m.Attribute.RelativeFilePath}typenamesrm" : null)) : null;
        byte[]? bytesGetCultureName = m.IsSRM ? Encoding.UTF8.GetBytes(GetRandomGetMethodName(Deterministic ? $"{m.Attribute.RelativeFilePath}bytesculturename" : null)) : null;
        byte[]? bytesGetString = m.IsSRM ? Encoding.UTF8.GetBytes(GetRandomGetMethodName(Deterministic ? $"{m.Attribute.RelativeFilePath}bytesstring" : null)) : null;

        var elements = GetXmlElementsByResXFilePath(m.Path);
        var items = DeserializeResXDataElements(elements).ToArray();
        Dictionary<string, string> getMethodNameDict = null!;
        KeyValuePair<string, RootDataXmlElement[]>[] t_items = null!;

        if (m.IsSRM)
        {
            var resxDir = Path.GetDirectoryName(m.Path);
            var resxFileNameWithoutEx = Path.GetFileNameWithoutExtension(m.Path);
            var t_items_query = from filePath in Directory.GetFiles(resxDir, $"{resxFileNameWithoutEx}.*.resx")
                                let cultureName = Path.GetFileNameWithoutExtension(filePath).TrimStart($"{resxFileNameWithoutEx}.")
                                where !cultureName.Contains('.') && IsCultureName(cultureName)
                                let els = GetXmlElementsByResXFilePath(filePath)
                                let els_class = DeserializeResXDataElements(els).ToArray()
                                select new KeyValuePair<string, RootDataXmlElement[]>(cultureName, els_class);
            t_items = t_items_query.ToArray();

            getMethodNameDict = items.ToDictionary(static x => x.Name, x => GetRandomGetMethodName(Deterministic ? x.Name : null));

            stream.Write(
"""
    static global::System.Globalization.CultureInfo? resourceCulture;

    /// <summary>
    ///   返回此类使用的缓存的 ResourceManager 实例。
    /// </summary>
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
    public static global::System.Resources.IStringResourceManager ResourceManager => (
"""u8);
            stream.Write(typeNameSRM!);
            stream.Write(
"""
)default;

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

    static int 
"""u8);
            stream.Write(bytesGetCultureName!);
            stream.Write(
"""
(CultureInfo? culture = null)
"""u8);
            if (t_items.Length == 0)
            {
                stream.Write(
"""
 => -1; // t_items.Length == 0
"""u8);
            }
            else
            {
                stream.Write(
"""

    {
        culture = GetCultureCore(culture);
        while (true)
        {
            if (culture == null || culture.LCID == 127 || string.IsNullOrWhiteSpace(culture.Name))
            {
                // LCID = 127 =	Invariant Language (Invariant Country)
                return -1;
            }
            else
            {
                var index = culture!.Name switch
                {

"""u8);
                for (int i = 0; i < t_items.Length; i++)
                {
                    var t_item = t_items[i];
                    stream.WriteFormat(
    """
                    "{0}" => {1},

"""u8, t_item.Key, i.ToString());
                }
                stream.Write(
"""
                    _ => -1,
                };
                if (index != -1)
                {
                    return index;
                }
                else if (culture.Parent == null)
                {
                    return -1;
                }
                else
                {
                    culture = culture.Parent;
                    continue;
                }
            }
        }
        static CultureInfo? GetCultureCore(CultureInfo? culture = null)
        {
            try
            {
                if (culture != null)
                    return culture;
                if (resourceCulture != null)
                    return resourceCulture;
                return CultureInfo.CurrentUICulture;
            }
            catch
            {
                return null;
            }
        }
    }
"""u8);
            }
            stream.Write(
"""


    readonly struct 
"""u8);
            stream.Write(typeNameSRM!);
            stream.Write(
"""
 : global::System.Resources.IStringResourceManager, global::System.Collections.Generic.IReadOnlyList<global::System.String>
    {
"""u8);

            #region StringResourceManager.GetString

            stream.Write(
"""

        public string GetString(string name, CultureInfo? culture = null) => 
"""u8);
            stream.WriteUtf16StrToUtf8OrCustom(m.TypeName);
            stream.Write(
"""
.
"""u8);
            stream.Write(bytesGetString!);
            stream.Write(
"""
(name, culture);

"""u8);

            #endregion

            #region StringResourceManager.SupportedUICultures

            stream.Write(
"""

        public global::System.Collections.Generic.IReadOnlyList<global::System.String> SupportedUICultures => (
"""u8);
            stream.Write(typeNameSRM!);
            stream.Write(
"""
)default;

"""u8);

            #endregion

            #region StringResourceManager.GetCultureName

            stream.Write(
"""

        public string? GetCultureName(CultureInfo? culture = null)
        {
            var index = 
"""u8);
            stream.WriteUtf16StrToUtf8OrCustom(m.TypeName);
            stream.Write(
"""
.
"""u8);
            stream.Write(bytesGetCultureName!);
            stream.Write(
"""
(culture);
            if (index != -1)
            {
                return ((global::System.Collections.Generic.IReadOnlyList<global::System.String>)this)[index];
            }
            return null;
        }

"""u8);

            #endregion

            #region StringResourceManager.IReadOnlyList

            stream.Write(
"""

        /// <inheritdoc/>
        readonly global::System.Collections.Generic.IEnumerator<global::System.String> global::System.Collections.Generic.IEnumerable<global::System.String>.GetEnumerator() => GetEnumerator().GetEnumerator();

        /// <inheritdoc/>
        readonly global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator() => GetEnumerator().GetEnumerator();

        /// <inheritdoc/>
        readonly int global::System.Collections.Generic.IReadOnlyCollection<global::System.String>.Count => 
"""u8);
            stream.WriteUtf16StrToUtf8OrCustom(t_items.Length.ToString());
            stream.Write(
"""
;

        /// <inheritdoc/>
        readonly string global::System.Collections.Generic.IReadOnlyList<global::System.String>.this[int index] => index switch
        {
"""u8);
            for (int i = 0; i < t_items.Length; i++)
            {
                var t_item = t_items[i];
                stream.WriteFormat(
"""

            {0} => "{1}",
"""u8, i.ToString(), t_item.Key);
            }
            stream.Write(
"""

            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null!),
        };

        static global::System.Collections.Generic.IEnumerable<global::System.String> GetEnumerator()
        {
"""u8);
            if (t_items.Length == 0)
            {
                stream.Write(
"""

            return global::System.Linq.Enumerable.Empty<global::System.String>();
"""u8);
            }
            else
            {
                foreach (var t_item in t_items)
                {
                    stream.WriteFormat(
"""

            yield return "{0}";
"""u8, t_item.Key);
                }
            }
            stream.Write(
"""

        }

"""u8);

            #endregion

            stream.Write(
"""
    }

"""u8);
        }
        else
        {
            stream.WriteFormat(
"""
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
        }
        stream.WriteNewLine();
        stream.WriteNewLine();

        if (m.IsSRM)
        {
            stream.Write(
"""
    static string 
"""u8);
            stream.Write(bytesGetString!);
            stream.Write(
"""
(string name, CultureInfo? culture = null) => name switch
    {

"""u8);
            foreach (var item in items)
            {
                stream.WriteFormat(
"""
        nameof({0}) => {1}(culture),

"""u8, item.Name, getMethodNameDict[item.Name]);
            }
            stream.Write(
"""
        _ => "",
    };


"""u8);

            foreach (var item in items)
            {
                stream.WriteFormat(
"""
    static string {0}(CultureInfo? culture = null) => 
"""u8, getMethodNameDict[item.Name]);
                if (t_items.Length == 0)
                {
                    stream.WriteNewLine();
                    WriteResStringValue(stream, item.Value);
                    stream.Write(
"""
;


"""u8);
                    continue;
                }
                stream.Write(bytesGetCultureName!);
                stream.WriteFormat(
"""
(culture) switch
"""u8, getMethodNameDict[item.Name], bytesGetCultureName);
                stream.Write(
"""

    {

"""u8);
                for (int i = 0; i < t_items.Length; i++)
                {
                    var t_item = t_items[i];
                    var t_item_xml = t_item.Value.FirstOrDefault(x => x.Name == item.Name);
                    if (t_item_xml != null && t_item_xml.Value != item.Value)
                    {
                        // 写入翻译的 resx 值
                        stream.Write(
"""
        
"""u8);
                        stream.WriteUtf16StrToUtf8OrCustom(i.ToString());
                        stream.Write(
"""
 => // 
"""u8);
                        stream.WriteUtf16StrToUtf8OrCustom(t_item.Key);
                        string cultureDisplayName = null!;
                        try
                        {
                            var culture = new CultureInfo(t_item.Key);
                            if (culture.LCID != 127 && !string.IsNullOrWhiteSpace(culture.Name) && !string.IsNullOrWhiteSpace(culture.DisplayName))
                            {
                                cultureDisplayName = culture.DisplayName;
                            }
                        }
                        catch { }
                        if (cultureDisplayName != null)
                        {
                            stream.Write(" "u8);
                            stream.WriteUtf16StrToUtf8OrCustom(cultureDisplayName);
                        }
                        stream.WriteNewLine();
                        WriteResStringValue(stream, t_item_xml.Value);
                        stream.Write(","u8);
                        stream.WriteNewLine();
                    }
                }

                stream.Write(
"""
        _ => 

"""u8);
                WriteResStringValue(stream, item.Value);
                stream.Write(","u8);
                stream.WriteFormat(
"""

    };


"""u8);
            }
        }

        foreach (var item in items)
        {
            item.WriteSummary(stream);
            if (m.IsSRM)
            {
                stream.WriteFormat(
"""
    public static string {0} => {1}(resourceCulture);
"""u8, item.Name, getMethodNameDict[item.Name]);
            }
            else
            {
                stream.WriteFormat(
"""
    public static string {0} => ResourceManager.GetString("{0}", resourceCulture) ?? "";
"""u8, item.Name);
            }
            stream.WriteNewLine();
            stream.WriteNewLine();
        }

        stream.WriteCurlyBracketRight(); // }
        stream.WriteNewLine();
    }

    static void WriteResStringValue(Stream stream, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            if (value.Contains("\"\"\""))
            {
                stream.Write("\"\""u8);
                var bytes = Encoding.Unicode.GetBytes(value);
                for (var i = 0; i < bytes.Length; i += 2)
                {
                    stream.Write(@"\u"u8);
                    stream.WriteUtf16StrToUtf8OrCustom(bytes[i + 1].ToString("x2"));
                    stream.WriteUtf16StrToUtf8OrCustom(bytes[i].ToString("x2"));
                }
                stream.Write("\"\""u8);
            }
            else
            {
                var y3 = "\"\"\""u8;
                stream.Write(y3);
                stream.WriteNewLine();
                stream.WriteUtf16StrToUtf8OrCustom(value);
                stream.WriteNewLine();
                stream.Write(y3);
            }
        }
        else
        {
            stream.Write("\"\""u8);
        }
    }
}
