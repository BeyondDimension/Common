
namespace BD.Common8.SourceGenerator.Bcl.Templates;

#pragma warning disable RS1035 // 不要使用禁用于分析器的 API

/// <summary>
/// 用于标注需要根据路径生成常量的源生成器与模板
/// </summary>
[Generator]
public sealed class ConstantsByPathTemplate :
    GeneratedAttributeTemplateBase<
        ConstantsByPathGeneratedAttribute,
        ConstantsByPathTemplate.SourceModel>
{
    protected override string Id =>
        "ConstantsByPath";

    protected override string AttrName =>
        "System.Runtime.CompilerServices.ConstantsByPathGeneratedAttribute";

    protected override IEnumerable<ConstantsByPathGeneratedAttribute>? GetMultipleAttributes(ImmutableArray<AttributeData> attributes)
    {
        var items = attributes.Where(x => x.ClassNameEquals(AttrName));
        foreach (var attribute in items)
        {
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

        /// <summary>
        /// 文件夹路径
        /// </summary>
        public required string DirPath { get; init; }

        /// <inheritdoc cref="ConstantsByPathGeneratedAttribute"/>
        public required ConstantsByPathGeneratedAttribute Attribute { get; init; }
    }

    protected override SourceModel GetSourceModel(GetSourceModelArgs args)
    {
        var dirPath = Path.GetFullPath(
            Path.Combine(
                [Path.GetDirectoryName(args.m.SemanticModel.SyntaxTree.FilePath),
                    ..
                args.attr.RelativePath.Split('\\')]));

        SourceModel model = new()
        {
            Namespace = args.@namespace,
            TypeName = args.typeName,
            Attribute = args.attr,
            DirPath = dirPath,
        };
        return model;
    }

    protected override void WriteFile(Stream stream, SourceModel m)
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
    /// <summary>
    /// {1}
    /// </summary>
    public const string {0}
"""u8, m.Attribute.NamePrefix, relativePathStr);
                WriteVariableName(stream, relativePath);

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
}
