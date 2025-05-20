#pragma warning disable RS1035 // 不要使用禁用于分析器的 API
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.IO;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace BD.Common8.SourceGenerator.Templates.Abstractions;

/// <summary>
/// 模板基类
/// </summary>
public abstract class TemplateBase
{
    readonly Lazy<bool> mIsDesignTimeBuild = new(static () =>
    {
        // build C:\Program Files\Microsoft Visual Studio\2022\Preview\MSBuild\Current\Bin\Roslyn\csc.exe .NET Framework 4.7.2
        // design C:\Program Files\Microsoft Visual Studio\2022\Preview\Common7\ServiceHub\Hosts\ServiceHub.Host.dotnet.x64\ServiceHub.RoslynCodeAnalysisService.exe .NET 8.0
        try
        {
            var processPath =
#if NET6_0_OR_GREATER
                Environment.ProcessPath;
#else
                Environment2.ProcessPath;
#endif

            if (processPath != null)
            {
                var isWindows =
#if !NET5_0_OR_GREATER
                    OperatingSystem2.IsWindows();
#else
                    OperatingSystem.IsWindows();
#endif
                var indexD = isWindows ? processPath.LastIndexOf('.') : processPath.Length - 1;
                var indexX = processPath.LastIndexOf(Path.DirectorySeparatorChar);
                if (indexD > indexX)
                {
                    var fileNameWithoutExtension = processPath.AsSpan().Slice(indexX + 1, indexD - indexX - 1);
                    if (fileNameWithoutExtension.Equals("ServiceHub.RoslynCodeAnalysisService".AsSpan(), StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
        }
        catch
        {
        }
        return false;
    }, LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// 是否在 IDE 设计器时生成，例如当前程序集在进程 ServiceHub.RoslynCodeAnalysisService.exe 中运行时为 <see langword="true"/>，在 csc.exe 中运行则为 <see langword="false"/>
    /// </summary>
    protected bool IsDesignTimeBuild => mIsDesignTimeBuild.Value;

    /// <summary>
    /// 确定性编译
    /// <para>https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/compiler-options/code-generation#deterministic</para>
    /// </summary>
    protected bool Deterministic { get; set; }

    public static string? GetAssemblyInformationalVersion(Assembly assembly)
    {
        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        if (string.IsNullOrWhiteSpace(version))
        {
            return null;
        }
        else
        {
            var array = version!.Split(['+',], StringSplitOptions.RemoveEmptyEntries);
            return array.Length switch
            {
                1 => version,
                _ => $"{array[0]} ({(array[1].Length > 8 ? array[1][..8] : array[1])})",
            };
        }
    }

    public static string? GetAssemblyFrameworkDisplayNameOrName(Assembly assembly)
    {
        var attr = assembly.GetCustomAttribute<global::System.Runtime.Versioning.TargetFrameworkAttribute>();
        var frameworkDisplayName = attr?.FrameworkDisplayName;
        if (!string.IsNullOrWhiteSpace(frameworkDisplayName))
        {
            return frameworkDisplayName;
        }
        var frameworkName = attr?.FrameworkName;
        if (!string.IsNullOrWhiteSpace(frameworkName))
        {
            return frameworkName;
        }
        return null;
    }

    public static string? GetAssemblyLocation(Assembly assembly)
    {
        string? location;
        try
        {
            location = assembly.Location;
            string? userProfile;
            try
            {
                userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }
            catch
            {
                userProfile = null;
            }
            try
            {
                if (!string.IsNullOrWhiteSpace(userProfile))
                {
                    if (location.StartsWith(userProfile, StringComparison.OrdinalIgnoreCase))
                    {
                        // 隐藏用户名
                        location = $"%USERPROFILE%{location[userProfile!.Length..]}";
                    }
                }
            }
            catch
            {
            }
        }
        catch
        {
            location = null;
        }
        return location;
    }

    static readonly Lazy<(string? runtimeVersion, string? roslynVersion, string? thisVersion, string? processPath, string? frameworkDisplayNameOrName, string? location)> _VersionInfo = new(GetVersionInfo, true);

    static (string? runtimeVersion, string? roslynVersion, string? thisVersion, string? processPath, string? frameworkDisplayNameOrName, string? location) GetVersionInfo()
    {
        string? runtimeVersion, roslynVersion, thisVersion, frameworkDisplayNameOrName, location;
        try
        {
            runtimeVersion = GetAssemblyInformationalVersion(typeof(object).Assembly);
        }
        catch
        {
            runtimeVersion = Environment.Version.ToString();
        }
        try
        {
            roslynVersion = GetAssemblyInformationalVersion(typeof(Microsoft.CodeAnalysis.CSharp.LanguageVersion).Assembly);
        }
        catch
        {
            roslynVersion = null;
        }
        var thisAssembly = typeof(TemplateBase).Assembly;
        try
        {
            thisVersion = GetAssemblyInformationalVersion(thisAssembly);
        }
        catch
        {
            thisVersion = null;
        }
        try
        {
            frameworkDisplayNameOrName = GetAssemblyFrameworkDisplayNameOrName(thisAssembly);
        }
        catch
        {
            frameworkDisplayNameOrName = null;
        }
        location = GetAssemblyLocation(thisAssembly);
        var processPath =
#if NET6_0_OR_GREATER
            Environment.ProcessPath;
#else
            Environment2.ProcessPath;
#endif
        return (runtimeVersion, roslynVersion, thisVersion, processPath, frameworkDisplayNameOrName, location);
    }

    public static (string? runtimeVersion, string? roslynVersion, string? thisVersion, string? processPath, string? frameworkDisplayNameOrName, string? location) VersionInfo => _VersionInfo.Value;

    /// <summary>
    /// 写入文件头
    /// </summary>
    public static void WriteFileHeader(
        Stream stream,
        Type? generatorType = null)
    {
        (string? runtimeVersion, string? roslynVersion, string? thisVersion, string? processPath, string? frameworkDisplayNameOrName, string? location) = VersionInfo;
        stream.Write(
"""
﻿//------------------------------------------------------------------------------
// <auto-generated>

"""u8);
        var toolName = generatorType?.Assembly.FullName;
        if (string.IsNullOrWhiteSpace(toolName))
        {
            stream.Write(
"""
//     此代码由工具生成。
"""u8);
        }
        else
        {
            stream.Write(
"""
//     此代码由包 
"""u8);
            stream.WriteUtf16StrToUtf8OrCustom(toolName);
            stream.Write(
"""
 源生成。
"""u8);
        }
        stream.Write(
"""

//     生成器运行时版本：
"""u8);
        stream.WriteUtf16StrToUtf8OrCustom(runtimeVersion);
        stream.Write(
"""

//     编译器版本：
"""u8);
        stream.WriteUtf16StrToUtf8OrCustom(roslynVersion);
        stream.Write(
"""

//     生成器版本：
"""u8);
        stream.WriteUtf16StrToUtf8OrCustom(thisVersion);
        stream.Write(" "u8);
        stream.WriteUtf16StrToUtf8OrCustom(frameworkDisplayNameOrName);
        stream.Write(" "u8);
        stream.WriteUtf16StrToUtf8OrCustom(location);
        stream.Write(" -> "u8);
        var asmRecyclableMemoryStream = typeof(RecyclableMemoryStream).Assembly; // 测试 NuGet 包程序集依赖加载行为
        var location2 = GetAssemblyLocation(asmRecyclableMemoryStream) ?? asmRecyclableMemoryStream.ToString();
        stream.WriteUtf16StrToUtf8OrCustom(location2);
        stream.Write(
"""

//     生成器进程路径：
"""u8);
        stream.WriteUtf16StrToUtf8OrCustom(processPath);
        stream.Write(
"""

//     源生成模板类型：
"""u8);
        stream.WriteUtf16StrToUtf8OrCustom(generatorType?.FullName);
        stream.Write(
"""

//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------
// ReSharper disable once CheckNamespace
#nullable enable
#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE0005 // 删除不必要的 using 指令
#pragma warning disable IDE1006 // 命名样式
#pragma warning disable SA1209 // Using alias directives should be placed after other using directives
#pragma warning disable SA1211 // Using alias directives should be ordered alphabetically by alias name
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

"""u8);
    }

    /// <summary>
    /// 写入命名空间
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="namespace"></param>
    /// <param name="isFileNamespace"></param>
    /// <param name="isFirstWriteNamespace"></param>
    protected static void WriteNamespace(
        Stream stream,
        string @namespace,
        bool isFileNamespace = true,
        bool isFirstWriteNamespace = true)
    {
        if (isFirstWriteNamespace)
        {
            stream.Write(
"""
#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配

"""u8);
        }
        else
        {
            stream.WriteNewLine();
        }
        if (!string.IsNullOrWhiteSpace(@namespace))
        {
            stream.WriteFormat(
"""
﻿namespace {0}{1}
                

"""u8, @namespace, isFileNamespace ? ";" : "");
        }
    }

    static readonly Lazy<string> mFileVersion = new(() =>
    {
        var assembly = typeof(TemplateBase).Assembly;
        var fileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
        return fileVersion ?? string.Empty;
    });

    /// <summary>
    /// 获取当前源生成器的文件版本
    /// </summary>
    protected static string FileVersion => mFileVersion.Value;

    #region Random

    /// <inheritdoc cref="System.Random"/>
    protected static Random Random =>
#if NET6_0_OR_GREATER
        Random.Shared;
#else
        random ??= new(Guid.NewGuid().GetHashCode());

    static Random? random;
#endif

    const string random_chars = "_ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    static string ToStringWithGuid(char[] chars)
    {
        var guid = Guid.NewGuid().ToString("N");
        var chars_new = new char[chars.Length + guid.Length];
        int i = 0;
        for (; i < chars.Length; i++)
        {
            chars_new[i] = chars[i];
        }
        for (int j = 0; j < guid.Length; j++)
        {
            chars_new[i] = guid[j];
            i++;
        }
        return new string(chars_new);
    }

    /// <summary>
    /// 获取随机字段名
    /// </summary>
    /// <returns></returns>
    protected string GetRandomFieldName(string? key)
    {
        if (!string.IsNullOrWhiteSpace(key)) return "k__" + ComputeSHA256(key);
        var fieldName = "k__BackingField".ToCharArray();
        for (int i = 0; i < fieldName.Length / 2; i++)
        {
            var index = Random.Next(fieldName.Length);
            fieldName[index] = random_chars[Random.Next(random_chars.Length)];
        }
        var a = new string(fieldName);
        return ToStringWithGuid(fieldName);
    }

    /// <summary>
    /// 获取随机获取方法名
    /// </summary>
    /// <returns></returns>
    protected string GetRandomGetMethodName(string? key)
    {
        if (!string.IsNullOrWhiteSpace(key)) return "get_" + ComputeSHA256(key);
        const string random_chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
        var chars = new char[Random.Next(24, 48)];
        for (int i = 0; i < chars.Length; i++)
        {
            switch (i)
            {
                case 0:
                    chars[0] = 'g';
                    break;
                case 1:
                    chars[1] = 'e';
                    break;
                case 2:
                    chars[2] = 't';
                    break;
                case 3:
                    chars[3] = '_';
                    break;
                default:
                    chars[i] = random_chars[Random.Next(random_chars.Length)];
                    break;
            }
        }
        return ToStringWithGuid(chars);
    }

    /// <summary>
    /// 获取随机获取类名
    /// </summary>
    /// <returns></returns>
    protected string GetRandomClassName(string? key)
    {
        if (!string.IsNullOrWhiteSpace(key)) return "C" + ComputeSHA256(key);
        const string random_chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890_";
        var chars = new char[Random.Next(24, 48)];
        for (int i = 0; i < chars.Length; i++)
        {
            if (i == 0)
            {
                const string random_chars_0 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                chars[0] = random_chars_0[Random.Next(random_chars_0.Length)];
            }
            else
            {
                chars[i] = random_chars[Random.Next(random_chars.Length)];
            }
        }
        return ToStringWithGuid(chars);
    }

    protected string ComputeSHA256(string? key)
    {
#if NET5_0_OR_GREATER
        byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(key ?? ""));
#else
        using SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(key ?? ""));
#endif
        StringBuilder stringBuilder = new();
        foreach (var item in hashBytes)
        {
            stringBuilder.Append(item.ToString("X2"));
        }
        return stringBuilder.ToString();

    }

    /// <summary>
    /// 生成随机字符串，长度为固定传入字符串
    /// </summary>
    /// <param name="length">要生成的字符串长度</param>
    /// <param name="randomChars">随机字符串字符集</param>
    /// <returns></returns>
    protected static string GenerateRandomString(int length = 6,
        string randomChars = random_chars)
    {
        var random = Random;
        var result = new char[length];
        if (random.Next(256) % 2 == 0)
            for (var i = length - 1; i >= 0; i--) // 5 4 3 2 1 0
                EachGenerate(i);
        else
            for (var i = 0; i < length; i++) // 0 1 2 3 4 5
                EachGenerate(i);
        return new string(result);
        void EachGenerate(int i)
        {
            var index = random.Next(0, randomChars.Length);
            var temp = RandomCharAt(randomChars, index);
            static char RandomCharAt(string s, int index)
            {
                if (index == s.Length) index = 0;
                else if (index > s.Length) index %= s.Length;
                return s[index];
            }
            result[i] = temp;
        }
    }

    #endregion

    /// <summary>
    /// 写入变量名
    /// </summary>
    protected static void WriteVariableName(Stream stream, char[] chars, bool upper = true)
    {
        for (int i = 0; i < chars.Length; i++)
        {
            var item = chars[i];
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
                    chars[i] = char.ToUpperInvariant(item);
                }
                upper = false;
            }
            stream.Write(Encoding.UTF8.GetBytes(chars, i, 1));
        }
    }

    /// <summary>
    /// 获取字符串是否为有效的 <see cref="CultureInfo.Name"/>
    /// </summary>
    /// <param name="cultureName"></param>
    /// <returns></returns>
    protected static bool IsCultureName(string cultureName)
    {
        if (!string.IsNullOrWhiteSpace(cultureName))
        {
            try
            {
                var cultureInfo = new CultureInfo(cultureName);
                if (cultureInfo.DisplayName == cultureName &&
                    cultureInfo.EnglishName == cultureName &&
                    cultureInfo.IetfLanguageTag == cultureName &&
                    cultureInfo.Name == cultureName &&
                    cultureInfo.NativeName == cultureName &&
                    cultureInfo.TwoLetterISOLanguageName == cultureName)
                {
                    return false;
                }
                return true;
            }
            catch
            {
            }
        }
        return false;
    }
}

/// <summary>
/// 基于 GeneratedAttribute 的源生成模板
/// </summary>
/// <typeparam name="TGeneratedAttribute">生成特性模型</typeparam>
/// <typeparam name="TSourceModel">生成源文件参数模型</typeparam>
public abstract class GeneratedAttributeTemplateBase<TGeneratedAttribute, TSourceModel> : TemplateBase, IIncrementalGenerator
    where TGeneratedAttribute : notnull
    where TSourceModel : GeneratedAttributeTemplateBase<TGeneratedAttribute, TSourceModel>.ISourceModel
{
    /// <summary>
    /// 从源码中读取并分析生成器所需要的模型
    /// </summary>
    public interface ISourceModel
    {
        /// <summary>
        /// 命名空间
        /// </summary>
        string Namespace { get; }

        /// <summary>
        /// 类型名
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// 生成特性模型
        /// </summary>
        TGeneratedAttribute Attribute { get; }

        int I { get; }
    }

    /// <summary>
    /// typeof(TGeneratedAttribute).Name.TrimEnd("GeneratedAttribute")
    /// </summary>
    protected virtual string Id => typeof(TGeneratedAttribute).Name.TrimEnd("GeneratedAttribute");

    /// <summary>
    /// typeof(TGeneratedAttribute).FullName
    /// </summary>
    protected virtual string AttrName => typeof(TGeneratedAttribute).FullName!;

#if DEBUG
    /// <summary>
    /// 根据 <see cref="AttributeData"/> 还原 TGeneratedAttribute 数据
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    [Obsolete("use GetMultipleAttributes", true)]
    protected virtual TGeneratedAttribute? GetAttribute(ImmutableArray<AttributeData> attributes)
    {
        return GetMultipleAttributes(attributes).FirstOrDefault();
    }
#endif

    /// <summary>
    /// 根据 <see cref="AttributeData"/> 还原多个 TGeneratedAttribute 数据
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    protected virtual IEnumerable<TGeneratedAttribute> GetMultipleAttributes(ImmutableArray<AttributeData> attributes)
    {
        return [];
    }

#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
#pragma warning disable SA1604 // Element documentation should have summary

#pragma warning disable IDE1006 // 命名样式
    /// <summary>
    /// 获取 <see cref="ISourceModel"/> 需要的参数数据
    /// </summary>
    protected readonly record struct GetSourceModelArgs
    {
        /// <see cref="SourceProductionContext"/>
        public SourceProductionContext spc { get; init; }

        /// <see cref="GeneratorAttributeSyntaxContext"/>
        public GeneratorAttributeSyntaxContext m { get; init; }

        /// <see cref="INamedTypeSymbol"/>
        public INamedTypeSymbol symbol { get; init; }

        /// <summary>
        /// 命名空间
        /// </summary>
        public string @namespace { get; init; }

        /// <summary>
        /// 类型名
        /// </summary>
        public string typeName { get; init; }

        /// <summary>
        /// 调用 <see cref="GetMultipleAttributes"/> 的返回值
        /// </summary>
        public TGeneratedAttribute attr { get; init; }

        public readonly int i { get; init; }
    }
#pragma warning restore IDE1006 // 命名样式

#pragma warning restore SA1604 // Element documentation should have summary
#pragma warning restore SA1307 // Accessible fields should begin with upper-case letter

    /// <summary>
    /// 业务实现获取 <see cref="ISourceModel"/>
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    protected abstract TSourceModel GetSourceModel(in GetSourceModelArgs args);

    /// <summary>
    /// 是否跳过生成器执行函数
    /// </summary>
    protected bool IgnoreExecute { get; set; }

    /// <summary>
    /// 能否为一个程序元素指定多个指示属性实例
    /// </summary>
    protected bool AllowMultiple { get; private set; } /*=> ((AttributeUsageAttribute)Attribute.GetCustomAttribute(Type.GetType(AttrName), typeof(AttributeUsageAttribute))).AllowMultiple;*/

    /// <summary>
    /// 通用增量源生成器执行函数
    /// </summary>
    /// <param name="spc"></param>
    /// <param name="m"></param>
    protected virtual void Execute(SourceProductionContext spc, GeneratorAttributeSyntaxContext m)
    {
#if DEBUG
        var thisTypeName = GetType().Name;
        Console.WriteLine($"{thisTypeName} Execute");
#endif
        try
        {
            if (m.TargetSymbol is not INamedTypeSymbol symbol)
                return;

            var @namespace = symbol.ContainingNamespace.ToDisplayString();
            if (string.Equals("<global namespace>", @namespace, StringComparison.OrdinalIgnoreCase))
                @namespace = string.Empty;
            var typeName = symbol.Name;

            var attributes = GetMultipleAttributes(symbol.GetAttributes());

            int i = 0;
            foreach (var attr in attributes)
            {
                if (IgnoreExecute)
                    return;
                var model = GetSourceModel(new()
                {
                    spc = spc,
                    m = m,
                    symbol = symbol,
                    @namespace = @namespace,
                    typeName = typeName,
                    attr = attr,
                    i = i,
                });
                if (IgnoreExecute || model is null)
                    return;
                if (!AllowMultiple && i >= 1)
                    AllowMultiple = true;
                ExecuteCore(spc, model);
                i++;
            }
        }
#pragma warning disable CS0168 // 声明了变量，但从未使用过
        catch (Exception ex)
#pragma warning restore CS0168 // 声明了变量，但从未使用过
        {
#if DEBUG
            Console.WriteLine(ex);
#endif
        }
    }

#if DEBUG
    void ConsoleWriteSourceText(string sourceTextString)
    {
        var thisTypeName = GetType().Name;
        Console.WriteLine();
        Console.WriteLine($"{thisTypeName}: ");
        Console.WriteLine(sourceTextString);

        switch (FileId) // 在 case 断点查看生成的源码字符串
        {
            case "ConstantsByPath":
                break;
            case "CopyProperties":
                break;
            case "SettingsProperty":
                break;
            case "SingletonPartition":
                break;
            case "ViewModelWrapper":
                break;
            case "IpcClient":
                break;
            case "IpcServer":
                break;
            case "Designer": // ResXGeneratedCodeAttribute
                break;
        }
    }
#endif

    static readonly RecyclableMemoryStreamManager manager = new();

    /// <summary>
    /// 通用增量源生成器执行函数
    /// </summary>
    /// <param name="spc"></param>
    /// <param name="m"></param>
    protected virtual void ExecuteCore(SourceProductionContext spc, in TSourceModel m)
    {
#if DEBUG
        var thisTypeName = GetType().Name;
        Console.WriteLine($"{thisTypeName} ExecuteCore");
#endif
        SourceText sourceText;
        try
        {
            // 使用 RecyclableMemoryStreamManager 替代 MemoryStream 节省内存
            // https://github.com/microsoft/Microsoft.IO.RecyclableMemoryStream
            using var memoryStream = manager.GetStream();
            try
            {
                WriteFile(memoryStream, m);
                if (memoryStream.Length == 0)
                    return;
            }
            catch (OperationCanceledException)
            {
#if DEBUG
                Console.WriteLine($"{thisTypeName} OperationCanceledException");
#endif
                return;
            }
            sourceText = SourceText.From(memoryStream, Encoding.UTF8, canBeEmbedded: true);
#if DEBUG
            ConsoleWriteSourceText(sourceText.ToString());
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
            var sourceTextString = builder.ToString();
            sourceText = SourceText.From(sourceTextString, Encoding.UTF8);
#if DEBUG
            ConsoleWriteSourceText(sourceTextString);
#endif
        }
        var hintName = $"{(string.IsNullOrEmpty(m.Namespace) ? "global_namespace" : m.Namespace.TrimStart("BD.Common8.SourceGenerator."))}.{m.TypeName}.{FileId}{(AllowMultiple ? "." + m.I : "")}.g.cs";
        spc.AddSource(hintName, sourceText);
    }

    /// <summary>
    /// 该模板生成源文件名中的唯一名称，默认使用 <see cref="Id"/>，可重写替换
    /// </summary>
    protected virtual string FileId => Id;

    /// <summary>
    /// 源生成器写入文件流执行逻辑
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="m"></param>
    protected abstract void WriteFile(Stream stream, in TSourceModel m);

    /// <inheritdoc/>
    void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context)
    {
        try
        {
            var option = context.AnalyzerConfigOptionsProvider
                .Select((options, _) =>
                {
                    options.GlobalOptions.TryGetValue("build_property.Deterministic", out var value);
                    return value;
                });
            var fullyQualifiedMetadataName = AttrName;
            var attrsource = context.SyntaxProvider.ForAttributeWithMetadataName(
                fullyQualifiedMetadataName,
                static (_, _) => true,
                static (content, _) => content);
            var source = attrsource.Combine(option);
            context.RegisterSourceOutput(source, (ctx, paris) =>
            {
                Deterministic = bool.TryParse(paris.Right, out var result) && result;
                Execute(ctx, paris.Left);
            });
#if DEBUG
            var thisTypeName = GetType().Name;
            Console.WriteLine($"{thisTypeName} Initialized, AttrName: {AttrName}, Id: {Id}, FileId: {FileId}.");
#endif
        }
#pragma warning disable CS0168 // 声明了变量，但从未使用过
        catch (Exception ex)
#pragma warning restore CS0168 // 声明了变量，但从未使用过
        {
#if DEBUG
            Console.WriteLine(ex);
#endif
        }
    }
}