namespace BD.Common8.SourceGenerator.Bcl.Templates;

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable RS1035 // 不要使用禁用于分析器的 API

sealed class ConstantsByPathTemplate : TemplateBase
{
    const string Id = "ConstantsByPath";

    public const string AttrName =
        $"System.Runtime.CompilerServices.{Id}GeneratedAttribute";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ConstantsByPathGeneratedAttribute GetAttribute(ImmutableArray<AttributeData> attributes)
    {
        var attribute = attributes.FirstOrDefault(static x => x.ClassNameEquals(AttrName));
        attribute.ThrowIsNull();

        string? relativePath = null, valuePrefix = null, namePrefix = null;
        for (int i = 0; i < attribute.ConstructorArguments.Length; i++)
        {
            var value = attribute.ConstructorArguments[i].GetObjectValue();
            switch (i)
            {
                case 0:
                    relativePath = value?.ToString();
                    break;
                case 1:
                    valuePrefix = value?.ToString();
                    break;
                case 2:
                    namePrefix = value?.ToString();
                    break;
            }
        }
        ConstantsByPathGeneratedAttribute result = new(relativePath.ThrowIsNull(), valuePrefix, namePrefix);

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

        /// <summary>
        /// 文件夹路径
        /// </summary>
        public required string DirPath { get; init; }

        /// <inheritdoc cref="ConstantsByPathGeneratedAttribute"/>
        public required ConstantsByPathGeneratedAttribute Attribute { get; init; }
    }

    public static void Execute(SourceProductionContext spc, GeneratorAttributeSyntaxContext m)
    {
        if (m.TargetSymbol is not INamedTypeSymbol symbol)
            return;

        var @namespace = symbol.ContainingNamespace.ToDisplayString();
        var typeName = symbol.Name;

        var attr = GetAttribute(symbol.GetAttributes());
        var path = Path.GetFullPath(Path.Combine([Path.GetDirectoryName(m.SemanticModel.SyntaxTree.FilePath), .. attr.RelativePath.Split('\\')]));

        SourceModel model = new()
        {
            Namespace = @namespace,
            TypeName = typeName,
            Attribute = attr,
            DirPath = path,
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
            stream.WriteFormat(
"""
partial class {0}
"""u8, m.TypeName);
            stream.WriteNewLine();
            stream.WriteCurlyBracketLeft(); // {
            stream.WriteNewLine();

            #region Body

            EachDirectories(m.DirPath);

            void EachFiles(params string[] files)
            {
                foreach (var file in files)
                {
                    var relativePathStr = file.TrimStart(m.DirPath);
                    var relativePath = relativePathStr.ToCharArray();

                    stream.WriteFormat(
"""
    public const string {0}
"""u8, m.Attribute.NamePrefix);
                    bool upper = true;
                    for (int i = 0; i < relativePath.Length; i++)
                    {
                        var item = relativePath[i];
                        if (i == 0 && item == Path.DirectorySeparatorChar)
                            continue;

                        if (item == Path.DirectorySeparatorChar)
                        {
                            upper = true;
                            stream.Write("_"u8);
                            continue;
                        }

                        if (item == '-' || item == '_')
                        {
                            upper = true;
                            continue;
                        }

                        if (item == '.')
                            break; // 跳过扩展名，不允许文件名相同扩展名不同的资源

                        if (upper)
                        {
                            if (!char.IsUpper(item))
                            {
                                relativePath[i] = item = char.ToUpperInvariant(item);
                            }
                            upper = false;
                        }
                        stream.Write(Encoding.UTF8.GetBytes(relativePath, i, 1));
                    }

                    stream.Write(
"""
 = $"{
"""u8);
                    stream.WriteUtf16StrToUtf8OrCustom(string.IsNullOrWhiteSpace(m.Attribute.ValuePrefix) ? "\"\"" : m.Attribute.ValuePrefix!);

                    stream.Write(
"""
}
"""u8);
                    relativePath = relativePathStr.ToCharArray();
                    for (int i = 0; i < relativePath.Length; i++)
                    {
                        var item = relativePath[i];
                        if (i == 0 && item == Path.DirectorySeparatorChar)
                            continue;

                        if (item == Path.DirectorySeparatorChar && Path.DirectorySeparatorChar != '/')
                            relativePath[i] = item = '/';

                        stream.Write(Encoding.UTF8.GetBytes(relativePath, i, 1));
                    }

                    stream.Write(
"""
";
"""u8);
                    stream.WriteNewLine();
                }
            }
            void EachDirectories(params string[] directories)
            {
                foreach (var dir in directories)
                {
                    EachFiles(Directory.GetFiles(dir));
                    EachDirectories(Directory.GetDirectories(dir));
                }
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
