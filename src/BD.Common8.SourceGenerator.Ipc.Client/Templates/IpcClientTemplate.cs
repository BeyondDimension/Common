namespace BD.Common8.SourceGenerator.Ipc.Templates;

/// <summary>
/// 用于 Ipc 客户端调用的源文件模板
/// </summary>
[Generator]
public sealed class IpcClientTemplate : IpcTemplateBase
{
    /// <inheritdoc/>
    protected override string FileId => "IpcClient";

    /// <inheritdoc/>
    protected override ServiceContractImplAttribute GetAttribute(ImmutableArray<AttributeData> attributes)
    {
        var attr = base.GetAttribute(attributes);
        switch (attr.GeneratorType)
        {
            case IpcGeneratorType.ClientWebApi:
            case IpcGeneratorType.ClientSignalR:
                break;
            default:
                IgnoreExecute = true; // 非客户端生成类型直接跳过
                return null!;
        }
        return attr;
    }

    /// <summary>
    /// 启用 bool 值小写字符串
    /// </summary>
    bool EnableBoolToLowerString => true;

    /// <inheritdoc/>
    protected override void WriteFile(Stream stream, SourceModel m)
    {
        WriteFileHeader(stream);
        stream.WriteNewLine();
        WriteNamespace(stream, m.Namespace);
        stream.WriteNewLine();
        stream.WriteFormat(
"""
sealed partial class {0} : {1}
"""u8, m.TypeName, m.Attribute.ServiceType);
        stream.WriteNewLine();
        stream.WriteCurlyBracketLeft();
        stream.WriteNewLine();
        stream.WriteFormat(
"""
    protected sealed override string HubName => "/Hubs/{0}";
"""u8, m.Attribute.ServiceType);
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
    public IAsyncEnumerable<{0}> {1}(
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
    public async Task<ApiRspImpl> {0}(
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
    public async Task<ApiRspImpl<{0}>> {1}(
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

            void WriteMethodBodyWithWebApi()
            {
                if (category == MethodParametersCategory.SimpleTypes)
                {
                    stream.WriteFormat(
"""
        string requestUri = $"/{0}/{1}
"""u8, m.Attribute.ServiceType, method.Name);
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
                        stream.Write("/{WebUtility.UrlEncode("u8);
                        if (paraType.TypeSymbol != null && paraType.TypeSymbol.IsEnum())
                        {
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
                            stream.WriteFormat(
"""
{0}.ToLowerString()
"""u8, paraName);
                        }
                        else if (paraType.IsSystemString)
                        {
                            stream.WriteFormat(
"""
{0}
"""u8, paraName);
                        }
                        else if (paraType.IsSystemDateOnly || paraType.IsSystemDateTime || paraType.IsSystemDateTimeOffset)
                        {
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
                            stream.WriteFormat(
"""
{0}.ToString()
"""u8, paraName);
                        }
                        stream.Write(")}"u8);
                    }
                }
                else
                {
                    stream.WriteFormat(
"""
        const string requestUri = "/{0}/{1}
"""u8, m.Attribute.ServiceType, method.Name);
                }

                stream.Write(
"""
";

"""u8);
                var requestMethod = GetRequestMethod(category);
                stream.WriteFormat(
"""
        var requestMethod = HttpMethod.{0};

"""u8, requestMethod);

                stream.Write(
"""
        WebApiClientSendArgs args = new(requestUri)
        {
            Method = requestMethod,
        };

"""u8);

                if (isApiRspImplByReturnType)
                {
                    stream.Write(
"""
        var result = await SendAsync<ApiRspImpl
"""u8);
                }
                else if (isAsyncEnumerableByReturnType)
                {
                    stream.WriteFormat(
"""
        var result = SendAsAsyncEnumerable<{0}
"""u8, returnType.GenericT);
                }
                else
                {
                    stream.WriteFormat(
"""
        var result = await SendAsync<ApiRspImpl<{0}>
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
                        break;
                }

                stream.Write(
"""
);
        return result!;

"""u8);
            }

            void WriteMethodBodyWithSignalR()
            {
                stream.WriteFormat(
"""
        const string methodName = "{0}
"""u8, method.Name);

                stream.Write(
"""
";

"""u8);

                if (isApiRspImplByReturnType)
                {
                    stream.Write(
"""
        var result = await HubSendAsync<ApiRspImpl
"""u8);
                }
                else if (isAsyncEnumerableByReturnType)
                {
                    stream.WriteFormat(
"""
        var result = HubSendAsAsyncEnumerable<{0}
"""u8, returnType.GenericT);
                }
                else
                {
                    stream.WriteFormat(
"""
        var result = await HubSendAsync<ApiRspImpl<{0}>
"""u8, returnType);
                }

                stream.Write(
"""
>(methodName, [
"""u8);

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
                    WriteMethodBodyWithWebApi();
                    break;
                case IpcGeneratorType.ClientSignalR:
                    WriteMethodBodyWithSignalR();
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
