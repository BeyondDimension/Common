using BD.Common8.Ipc.Attributes;
using BD.Common8.Ipc.Enums;
using BD.Common8.SourceGenerator.Helpers;
using BD.Common8.SourceGenerator.Ipc.Enums;
using BD.Common8.SourceGenerator.Ipc.Templates.Abstractions;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace BD.Common8.SourceGenerator.Ipc.Templates;

/// <summary>
/// 用于 Ipc 客户端调用的源文件模板
/// </summary>
[Generator]
public sealed class IpcClientTemplate : IpcTemplateBase
{
    /// <inheritdoc/>
    protected override string FileId => "IpcClient";

    protected override IEnumerable<ServiceContractImplAttribute> GetMultipleAttributes(ImmutableArray<AttributeData> attributes)
    {
        var items = base.GetMultipleAttributes(attributes);
        foreach (var attribute in items)
        {
            var isBreak = false;
            switch (attribute.GeneratorType)
            {
                case IpcGeneratorType.ClientWebApi:
                case IpcGeneratorType.ClientSignalR:
                    break;
                default:
                    isBreak = true; // 非客户端生成类型直接跳过
                    break;
            }
            if (!isBreak)
                yield return attribute;
        }
    }

    /// <summary>
    /// 启用 bool 值小写字符串
    /// </summary>
    bool EnableBoolToLowerString => true;

    //    static void WriteUsings(Stream stream)
    //    {
    //        stream.Write(
    //"""
    //using global::System.Extensions;


    //"""u8);
    //    }

    /// <inheritdoc/>
    protected override void WriteFile(Stream stream, in SourceModel m)
    {
        WriteFileHeader(stream);
        stream.WriteNewLine();
        //WriteUsings(stream);
        WriteNamespace(stream, m.Namespace);
        stream.WriteNewLine();
        stream.WriteFormat(
"""
sealed partial class {0}(global::BD.Common8.Ipc.Services.IIpcClientService ipcClientService) : {1}
"""u8, m.TypeName, m.Attribute.ServiceType);
        stream.WriteNewLine();
        stream.WriteCurlyBracketLeft();
        stream.WriteNewLine();
        stream.Write(
"""
    readonly global::BD.Common8.Ipc.Services.IIpcClientService ipcClientService = ipcClientService;
"""u8);
        stream.WriteNewLine();
        stream.WriteNewLine();

        foreach (var method in m.Methods)
        {
            var methodParas = GetMethodParas(method,
                out var category);
            switch (category)
            {
                case MethodParametersCategory.None:
                case MethodParametersCategory.SimpleTypes:
                case MethodParametersCategory.FromBody:
                case MethodParametersCategory.GeneratorModelFromBody:
                    break;
                default:
                    continue;
            }

            var returnType = GetReturnType(method,
                out var isApiRspImplByReturnType,
                out var isAsyncEnumerableByReturnType);

            if (isAsyncEnumerableByReturnType)
            {
                stream.WriteFormat(
"""
    public global::System.Collections.Generic.IAsyncEnumerable<{0}> {1}(
"""u8, returnType.GenericT, method.Name);
                WriteParameters();
                stream.Write(
"""
)
"""u8);
            }
            else if (isApiRspImplByReturnType)
            {
                stream.WriteFormat(
"""
    public async Task<global::BD.Common8.Models.ApiRspImpl> {0}(
"""u8, method.Name);
                WriteParameters();
                stream.Write(
"""
)
"""u8);
            }
            else
            {
                stream.WriteFormat(
"""
    public async Task<global::BD.Common8.Models.ApiRspImpl<{0}>> {1}(
"""u8, returnType, method.Name);

                WriteParameters();
                stream.Write(
"""
)
"""u8);
            }

            void WriteParameters()
            {
                for (int i = 0; i < methodParas.Length; i++)
                {
                    var (paraType, _, paraNameWithDefaultValue) = methodParas[i];
                    if (i == 0)
                    {
                        stream.WriteFormat(
"""
{0} {1}
"""u8, paraType, paraNameWithDefaultValue);
                    }
                    else
                    {
                        stream.WriteFormat(
"""
, {0} {1}
"""u8, paraType, paraNameWithDefaultValue);
                    }
                }
            }

            stream.WriteNewLine();
            stream.Write(
"""
    {
"""u8);
            stream.WriteNewLine();

            void WriteMethodBodyWithWebApiBySimpleTypesImplFormattableString(in SourceModel m)
            {
                stream.Write(
"""
        string requestUri = 
"""u8);
                WriteServiceTypeAndMethodName(stream, m.Attribute.ServiceType, method);
                for (int i = 0; i < methodParas.Length; i++)
                {
                    var (paraType, paraName, _) = methodParas[i];
                    if (i == methodParas.Length - 1)
                    {
                        if (paraType.IsSystemThreadingCancellationToken)
                        {
                            continue;
                        }
                    }

                    if (paraType.TypeSymbol != null && paraType.TypeSymbol.IsEnum())
                    {
                        stream.Write("/{("u8); // 枚举类型使用数值的值而非名称，所以也不需要 UrlEncode
                        string? enumUnderlyingType = null;
                        if (paraType.TypeSymbol is INamedTypeSymbol paraNamedType)
                        {
                            enumUnderlyingType = paraNamedType.EnumUnderlyingType?.ToDisplayString();
                        }
                        stream.WriteFormat(
"""
(({1}){0}).ToString()
"""u8, paraName, enumUnderlyingType ?? "int");
                    }
                    else if (EnableBoolToLowerString && paraType.IsSystemBoolean)
                    {
                        stream.Write("/{("u8); // bool 值固定小写字符串硬编码，不需要 UrlEncode
                        stream.WriteFormat(
"""
{0} ? "true" : "false"
"""u8, paraName);
                    }
                    else if (paraType.IsSystemString)
                    {
                        stream.Write("/{System.Net.WebUtility.UrlEncode("u8);
                        stream.WriteFormat(
"""
{0}
"""u8, paraName);
                    }
                    else if (paraType.IsSystemDateOnly || paraType.IsSystemDateTime || paraType.IsSystemDateTimeOffset)
                    {
                        stream.Write("/{System.Net.WebUtility.UrlEncode("u8);
                        // 日期时间类型需要使用往返（“O”、“o”）格式
                        // https://learn.microsoft.com/zh-cn/dotnet/standard/base-types/standard-date-and-time-format-strings#the-round-trip-o-o-format-specifier
                        // “O”或“o”标准格式说明符表示使用保留时区信息的模式的自定义日期和时间格式字符串，并发出符合 ISO8601 的结果字符串。
                        // 对于 DateTime 值，“O”或“o”标准格式说明符对应于“yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK”自定义格式字符串，对于 DateTimeOffset 值，“O”或“o”标准格式说明符则对应于“yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffzzz”自定义格式字符串。 在此字符串中，分隔各个字符（例如连字符、冒号和字母“T”）的单引号标记对指示各个字符是不能更改的文本。 撇号不会出现在输出字符串中。
                        stream.WriteFormat(
"""
{0}.ToString("O")
"""u8, paraName);
                    }
                    else
                    {
                        switch (paraType.GetTypeCode())
                        {
                            case TypeCode.Boolean:
                            case TypeCode.Byte:
                            case TypeCode.Decimal:
                            case TypeCode.Double:
                            case TypeCode.Int16:
                            case TypeCode.Int32:
                            case TypeCode.Int64:
                            case TypeCode.SByte:
                            case TypeCode.Single:
                            case TypeCode.UInt16:
                            case TypeCode.UInt32:
                            case TypeCode.UInt64:
                                stream.Write("/{("u8); // 数值类型不需要 UrlEncode
                                break;
                            default:
                                stream.Write("/{System.Net.WebUtility.UrlEncode("u8);
                                break;
                        }
                        stream.WriteFormat(
"""
{0}.ToString()
"""u8, paraName);
                    }
                    stream.Write(")}"u8);
                }
            }

            //            void WriteMethodBodyWithWebApiBySimpleTypesImplAddQueryString(in SourceModel m, ref bool isWritedRequestUriStringEnd)
            //            {
            //                stream.Write(
            //"""
            //        global::System.Collections.Generic.KeyValuePair<string, string>[] queryString = [

            //"""u8);
            //                for (int i = 0; i < methodParas.Length; i++)
            //                {
            //                    var (paraType, paraName, _) = methodParas[i];
            //                    if (i == methodParas.Length - 1)
            //                    {
            //                        if (paraType.IsSystemThreadingCancellationToken)
            //                        {
            //                            continue;
            //                        }
            //                        if (paraType.TypeSymbol != null && paraType.TypeSymbol.IsEnum())
            //                        {
            //                            string? enumUnderlyingType = null;
            //                            if (paraType.TypeSymbol is INamedTypeSymbol paraNamedType)
            //                            {
            //                                enumUnderlyingType = paraNamedType.EnumUnderlyingType?.ToDisplayString();
            //                            }
            //                            stream.WriteFormat(
            //"""
            //(({1}){0}).ToString()
            //"""u8, paraName, enumUnderlyingType ?? "int");
            //                        }
            //                    }
            //                }
            //                stream.Write(
            //"""
            //            new("", ""),
            //        ];
            //        var requestUri = global::System.String2.AddQueryString("", queryString);

            //"""u8);
            //                isWritedRequestUriStringEnd = true;
            //            }

            void WriteMethodBodyWithWebApi(in SourceModel m)
            {
                bool isWritedRequestUriStringEnd = false;
                if (category == MethodParametersCategory.SimpleTypes)
                {
                    WriteMethodBodyWithWebApiBySimpleTypesImplFormattableString(m);
                    //WriteMethodBodyWithWebApiBySimpleTypesImplAddQueryString(m, ref isWritedRequestUriStringEnd);
                }
                else
                {
                    stream.WriteFormat(
"""
        const string requestUri = 
"""u8);
                    WriteServiceTypeAndMethodName(stream, m.Attribute.ServiceType, method);
                }

                if (!isWritedRequestUriStringEnd)
                {
                    stream.Write(
"""
";

"""u8);
                }
                var requestMethod = GetRequestMethod(category);
                stream.WriteFormat(
"""
        var requestMethod = global::System.Net.Http.HttpMethod.{0};

"""u8, requestMethod);

                stream.Write(
"""
        global::BD.Common8.Http.ClientFactory.Models.WebApiClientSendArgs args = new(requestUri)
        {
            Method = requestMethod,
        };

"""u8);

                if (isApiRspImplByReturnType)
                {
                    stream.Write(
"""
        var result = await ipcClientService.SendAsync<global::BD.Common8.Models.ApiRspImpl
"""u8);
                }
                else if (isAsyncEnumerableByReturnType)
                {
                    stream.WriteFormat(
"""
        var result = ipcClientService.SendAsAsyncEnumerable<{0}
"""u8, returnType.GenericT);
                }
                else
                {
                    stream.WriteFormat(
"""
        var result = await ipcClientService.SendAsync<global::BD.Common8.Models.ApiRspImpl<{0}>
"""u8, returnType);
                }

                switch (category)
                {
                    case MethodParametersCategory.FromBody:
                        stream.WriteFormat(
"""
, {0}
"""u8, methodParas[0].ParaType);
                        break;
                    case MethodParametersCategory.GeneratorModelFromBody:
                        stream.Write(", "u8);
                        WriteTuple(stream, methodParas);
                        break;
                }

                stream.Write(
"""
>(args
"""u8);

                switch (category)
                {
                    case MethodParametersCategory.None:
                    case MethodParametersCategory.FromBody:
                        for (int i = 0; i < methodParas.Length; i++)
                        {
                            var (paraType, paraName, _) = methodParas[i];
                            if (i == methodParas.Length - 1)
                            {
                                if (paraType.IsSystemThreadingCancellationToken)
                                {
                                    stream.WriteFormat(
"""
, cancellationToken: {0}
"""u8, paraName);
                                    break;
                                }
                            }
                            stream.WriteFormat(
"""
, {0}
"""u8, paraName);
                        }
                        break;
                    case MethodParametersCategory.SimpleTypes:
                        if (methodParas.Length != 0)
                        {
                            var (paraType, paraName, _) = methodParas[^1];
                            if (paraType.IsSystemThreadingCancellationToken)
                            {
                                stream.WriteFormat(
"""
, cancellationToken: {0}
"""u8, paraName);
                            }
                        }
                        break;
                    case MethodParametersCategory.GeneratorModelFromBody:
                        stream.Write(
"""
, global::System.TupleExtensions.ToTuple<
"""u8);
                        for (int i = 0; i < methodParas.Length; i++)
                        {
                            var (paraType, _, _) = methodParas[i];
                            if (i == methodParas.Length - 1)
                            {
                                if (paraType.IsSystemThreadingCancellationToken)
                                {
                                    break;
                                }
                            }
                            if (i != 0)
                            {
                                stream.Write(
"""
, 
"""u8);
                            }
                            stream.WriteUtf16StrToUtf8OrCustom(paraType.ToString());
                        }
                        stream.Write(
"""
>((
"""u8);
                        for (int i = 0; i < methodParas.Length; i++)
                        {
                            var (paraType, paraName, _) = methodParas[i];
                            if (i == methodParas.Length - 1)
                            {
                                if (paraType.IsSystemThreadingCancellationToken)
                                {
                                    break;
                                }
                            }
                            if (i == 0)
                            {
                                stream.WriteUtf16StrToUtf8OrCustom(paraName);
                            }
                            else
                            {
                                stream.WriteFormat(
"""
, {0}
"""u8, paraName);
                            }
                        }
                        stream.Write(
"""
)), cancellationToken: cancellationToken
"""u8);
                        break;
                }

                stream.Write(
"""
);
        return result!;

"""u8);
            }

            void WriteMethodBodyWithSignalR(in SourceModel m)
            {
                stream.WriteFormat(
"""
        const string methodName = 
"""u8, m.Attribute.ServiceType, method.Name);
                WriteServiceTypeAndMethodName(stream, m.Attribute.ServiceType, method, separatorIs_: true);

                stream.Write(
"""
";

"""u8);

                if (isApiRspImplByReturnType)
                {
                    stream.Write(
"""
        var result = await ipcClientService.HubSendAsync<global::BD.Common8.Models.ApiRspImpl
"""u8);
                }
                else if (isAsyncEnumerableByReturnType)
                {
                    stream.WriteFormat(
"""
        var result = ipcClientService.HubSendAsAsyncEnumerable<{0}
"""u8, returnType.GenericT);
                }
                else
                {
                    stream.WriteFormat(
"""
        var result = await ipcClientService.HubSendAsync<global::BD.Common8.Models.ApiRspImpl<{0}>
"""u8, returnType);
                }

                stream.WriteFormat(
"""
>({0}, methodName, [
"""u8, string.IsNullOrWhiteSpace(m.Attribute.HubUrl) ? "null" : $"\"{m.Attribute.HubUrl}\"");

                for (int i = 0; i < methodParas.Length; i++)
                {
                    var (paraType, paraName, _) = methodParas[i];
                    if (i == methodParas.Length - 1)
                    {
                        if (paraType.IsSystemThreadingCancellationToken)
                            break;
                    }
                    stream.WriteFormat(i == 0 ?
"""
{0}
"""u8 :
"""
, {0}
"""u8, paraName);
                }

                stream.WriteFormat(
"""
], cancellationToken: cancellationToken);
        return result!;

"""u8);
            }

            switch (m.Attribute.GeneratorType)
            {
                case IpcGeneratorType.ClientWebApi:
                    WriteMethodBodyWithWebApi(m);
                    break;
                case IpcGeneratorType.ClientSignalR:
                    WriteMethodBodyWithSignalR(m);
                    break;
                default:
                    throw ThrowHelper.GetArgumentOutOfRangeException(m.Attribute.GeneratorType);
            }

            stream.Write(
"""
    }
"""u8);
            stream.WriteNewLine();
            stream.WriteNewLine();
        }

        stream.WriteCurlyBracketRight();
        stream.WriteNewLine();
    }
}
