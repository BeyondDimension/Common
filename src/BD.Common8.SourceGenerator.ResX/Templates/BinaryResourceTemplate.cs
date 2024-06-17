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

        var args = attribute.ThrowIsNull().ConstructorArguments[0].Value?.ToString();
        var appendTemplate = attribute.ConstructorArguments.Length >= 2 ? attribute.ConstructorArguments[1].Value?.ToString() : null;

        return new(args!, appendTemplate);
    }

    static bool TryGetValue<T>(NewtonsoftJsonObject obj, string propertyName, out T? value)
    {
        value = default;

        if (obj.TryGetValue(propertyName, out var jToken))
        {
            try
            {
                switch (jToken.Type)
                {
                    case JTokenType.Null:
                        break;
                    default:
                        value = jToken.Value<T>();
                        break;
                }
                return true;
            }
            catch
            {
            }
        }

        return false;
    }

    static IEnumerable<BinaryResourceFileInfo> Deserialize(string json)
    {
        var array = JArray.Parse(json).OfType<NewtonsoftJsonObject>();
        foreach (var item in array)
        {
            if (TryGetValue<string>(item, nameof(BinaryResourceFileInfo.Path), out var path))
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    TryGetValue<string>(item, nameof(BinaryResourceFileInfo.Type), out var type);
                    TryGetValue<string>(item, nameof(BinaryResourceFileInfo.Name), out var name);
                    TryGetValue<bool>(item, nameof(BinaryResourceFileInfo.Reverse), out var reverse);
                    yield return new(path!, (EGeneratedType)GetGeneratedType(type), name, reverse);
                }
            }
        }
    }

    public enum EGeneratedType : byte
    {
        ByteArray = 0,
        ReadOnlyMemoryStream = 1,
    }

    public const byte DefaultEGeneratedType = (byte)EGeneratedType.ByteArray;

    static byte GetGeneratedType(string? value)
    {
        if (value != null)
        {
            if (byte.TryParse(value, out var b) && Enum.IsDefined(typeof(EGeneratedType), (EGeneratedType)b))
            {
                return b;
            }
            else if (Enum.TryParse<EGeneratedType>(value, true, out var e))
            {
                return (byte)e;
            }
        }
        return DefaultEGeneratedType;
    }

    public sealed record class BinaryResourceFileInfo(string Path, EGeneratedType Type, string? Name, bool Reverse)
    {
        string? mFilePath;

        public string FilePath => mFilePath.ThrowIsNull();

        public BinaryResourceFileInfo SetFilePath(string value)
        {
            mFilePath = value;
            return this;
        }
    }

    static void WritePropertyName(BinaryResourceFileInfo fileInfo, Stream stream)
    {
        var propertyName = fileInfo.Name;
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            propertyName = Path.GetFileNameWithoutExtension(fileInfo.FilePath);
            var propertyNameCharArray = propertyName.ThrowIsNull().ToCharArray();
            WriteVariableName(stream, propertyNameCharArray);
        }
        else
        {
            stream.WriteUtf16StrToUtf8OrCustom(propertyName!);
        }
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
        public required BinaryResourceFileInfo[] FileInfos { get; init; }

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
        if (string.IsNullOrEmpty(args.attr.Arguments))
            return default;

        var queryFilePaths = from x in Deserialize(args.attr.Arguments)
                             let filePath = Path.GetFullPath(Path.Combine(
                                 [
                                     Path.GetDirectoryName(args.m.SemanticModel.SyntaxTree.FilePath),
                                     ..
                                     x.Path.Split('\\')
                                 ]))
                             select x.SetFilePath(filePath);

        SourceModel model = new()
        {
            NamedTypeSymbol = args.symbol,
            Attribute = args.attr,
            FileInfos = queryFilePaths.ToArray(),
            Namespace = args.@namespace,
            TypeName = args.typeName,
            IsPublic = false,
        };
        return model;
    }

    byte[] ReadAllBytes(string path)
    {
        if (IsDesignTimeBuild)
        {
            return [];
        }
        else
        {
            return File.ReadAllBytes(path);
        }
    }

    void WriteFileAllBytes(Stream stream, BinaryResourceFileInfo fileInfo)
    {
        var bytes = ReadAllBytes(fileInfo.FilePath);
        if (fileInfo.Reverse)
        {
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                WriteByte(i);
            }
        }
        else
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                WriteByte(i);
            }
        }
        void WriteByte(int i)
        {
            stream.Write("0x"u8);
            stream.WriteUtf16StrToUtf8OrCustom(bytes[i].ToString("X"));
            stream.Write(", "u8);
        }
    }

    protected override void WriteFile(Stream stream, SourceModel m)
    {
        if (m.FileInfos == null || m.FileInfos.Length == 0)
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

        var hasAppendTemplate = !string.IsNullOrWhiteSpace(m.Attribute.AppendTemplate);
        Dictionary<BinaryResourceFileInfo, string> propertyNameDict = null!;
        if (hasAppendTemplate)
        {
            propertyNameDict = m.FileInfos.ToDictionary(static x => x, static x => GetRandomGetMethodName());
        }

        foreach (var fileInfo in m.FileInfos)
        {
            if (!File.Exists(fileInfo.FilePath))
                continue;

            stream.Write(
"""
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
#if NET35 || NET40
    [global::System.Runtime.CompilerServices.MethodImpl((MethodImplOptions)0x100)]
#else
    [global::System.Runtime.CompilerServices.MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    static 
"""u8);
            switch (fileInfo.Type)
            {
                case EGeneratedType.ByteArray:
                    stream.Write(
"""
byte[] 
"""u8);
                    break;
                case EGeneratedType.ReadOnlyMemoryStream:
                    stream.Write(
"""
global::System.IO.ReadOnlyMemoryStream 
"""u8);
                    break;
            }

            if (hasAppendTemplate)
            {
                var propertyName = propertyNameDict[fileInfo];
                stream.WriteUtf16StrToUtf8OrCustom(propertyName!);
            }
            else
            {
                WritePropertyName(fileInfo, stream);
            }

            switch (fileInfo.Type)
            {
                case EGeneratedType.ByteArray:
                    {
                        stream.Write(
"""
() => [
"""u8);
                        WriteFileAllBytes(stream, fileInfo);
                        stream.Write(
"""
];


"""u8);
                    }
                    break;
                case EGeneratedType.ReadOnlyMemoryStream:
                    {
                        stream.Write(
"""
() => new([
"""u8);
                        WriteFileAllBytes(stream, fileInfo);
                        if (fileInfo.Reverse)
                        {
                            stream.Write(
"""
], true);


"""u8);
                        }
                        else
                        {
                            stream.Write(
"""
], false);


"""u8);
                        }
                    }
                    break;
            }
        }

        if (hasAppendTemplate)
        {
            var appendTemplate = m.Attribute.AppendTemplate!;
            using var propertyNameStream = new MemoryStream();

            foreach (var item in propertyNameDict)
            {
                propertyNameStream.Position = 0;
                WritePropertyName(item.Key, propertyNameStream);
                propertyNameStream.SetLength(propertyNameStream.Position);

                var propertyName = Encoding.UTF8.GetString(propertyNameStream.ToArray());
                appendTemplate = appendTemplate.Replace($"{{{propertyName}}}", item.Value);
            }

            stream.WriteUtf16StrToUtf8OrCustom(appendTemplate);
            stream.WriteNewLine();
        }

        #region }
        stream.WriteCurlyBracketRight(); // }
        stream.WriteNewLine();
        #endregion
    }
}