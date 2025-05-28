using BD.Common8.SourceGenerator.Helpers;
using BD.Common8.SourceGenerator.Templates.Abstractions;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json.Linq;
using System.Buffers;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Text;

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

    protected override IEnumerable<BinaryResourceAttribute> GetMultipleAttributes(ImmutableArray<AttributeData> attributes)
    {
        var items = attributes.Where(x => x.ClassNameEquals(AttrName));
        foreach (var attribute in items)
        {
            var args = attribute.ThrowIsNull().ConstructorArguments[0].Value?.ToString();
            var appendTemplate = attribute.ConstructorArguments.Length >= 2 ? attribute.ConstructorArguments[1].Value?.ToString() : null;

            yield return new(args!, appendTemplate);
        }
    }

    static bool TryGetValue<T>(JObject obj, string propertyName, out T? value)
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
        var array = JArray.Parse(json).OfType<JObject>();
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

        /// <inheritdoc cref="I"/>
        public required int I { get; init; }
    }

    protected override SourceModel GetSourceModel(in GetSourceModelArgs args)
    {
        if (string.IsNullOrEmpty(args.attr.Arguments))
            return default;

        var codeDirPath = Path.GetDirectoryName(args.m.SemanticModel.SyntaxTree.FilePath)!;
        var queryFilePaths = from x in Deserialize(args.attr.Arguments)
                             let filePath = Path.GetFullPath(Path.Combine(
                                 [
                                     codeDirPath,
                                     ..
                                     x.Path.Split('\\')
                                 ]))
                             select x.SetFilePath(filePath);

        SourceModel model = new()
        {
            I = args.i,
            NamedTypeSymbol = args.symbol,
            Attribute = args.attr,
            FileInfos = [.. queryFilePaths],
            Namespace = args.@namespace,
            TypeName = args.typeName,
            IsPublic = false,
        };
        return model;
    }

    static void WriteByte(Stream stream, byte b)
    {
        stream.Write("0x"u8);
        stream.WriteUtf16StrToUtf8OrCustom(b.ToString("X"));
        stream.Write(", "u8);
    }

    internal static bool WriteFile(Stream stream, string filePath, bool reverse)
    {
        bool fileExists = true;
        try
        {
            // 使用池化内存缓冲区分片读取文件流
            const int bufferSize = 4096; // 缓冲区大小
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            if (reverse) // 如果需要反转字节顺序
            {
                // len = 10, bufferSize = 4, forCount = 3
                var forCount = fileStream.Length / bufferSize;
                if (fileStream.Length % bufferSize != 0)
                {
                    // 除法余数进一
                    forCount++;
                }
                var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
                try
                {
                    while (forCount != 0) // 循环缓冲区计数
                    {
                        fileStream.Position = (forCount - 1) * bufferSize; // 计算当前读取的起始位置
                        // 反转时必须限定缓冲区长度与设定长度一致，如果缓冲区申请长度大于预设值，则忽略末尾部分
                        var count = fileStream.Read(buffer, 0, bufferSize);
                        if (count <= 0)
                        {
                            // 读取到文件末尾，跳出循环
                            break;
                        }

                        var bufferSpan = buffer.AsSpan(0, count);
                        for (int i = bufferSpan.Length - 1; i >= 0; i--)
                        {
                            WriteByte(stream, bufferSpan[i]);
                        }
                        forCount--; // 减少计数器
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
            else
            {
                var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
                try
                {
                    while (true)
                    {
                        var count = fileStream.Read(buffer, 0, buffer.Length);
                        if (count <= 0)
                        {
                            // 读取到文件末尾，跳出循环
                            break;
                        }

                        var bufferSpan = buffer.AsSpan(0, count);
                        for (int i = 0; i < bufferSpan.Length; i++)
                        {
                            WriteByte(stream, bufferSpan[i]);
                        }
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }
        catch (DirectoryNotFoundException)
        {
            fileExists = false;
        }
        catch (FileNotFoundException)
        {
            fileExists = false;
        }
        return fileExists;
    }

    void WriteFileAllBytes(Stream stream, BinaryResourceFileInfo fileInfo)
    {
        if (IsDesignTimeBuild)
        {
            stream.Write(
"""
/* NotImplemented.ByDesign */
"""u8);
            return;
        }

        bool fileExists = WriteFile(stream, fileInfo.FilePath, fileInfo.Reverse);
        if (!fileExists)
        {
            stream.Write(
"""
/* FileNotFound */
"""u8);
            return;
        }
    }

    protected override void WriteFile(Stream stream, in SourceModel m)
    {
        if (m.FileInfos == null || m.FileInfos.Length == 0)
            return;

        WriteFileHeader(stream, GetType());
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
            propertyNameDict = m.FileInfos.ToDictionary(static x => x, x => GetRandomGetMethodName(Deterministic ? x.FilePath : null));
        }
        foreach (var fileInfo in m.FileInfos)
        {
            stream.Write(
"""
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
#if NET35 || NET40
    [global::System.Runtime.CompilerServices.MethodImpl((global::System.Runtime.CompilerServices.MethodImplOptions)0x100)]
#else
    [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
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