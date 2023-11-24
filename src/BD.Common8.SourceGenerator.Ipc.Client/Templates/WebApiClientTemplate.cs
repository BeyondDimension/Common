namespace BD.Common8.SourceGenerator.Ipc.Templates;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 用于客户端调用的源文件模板
/// </summary>
[Generator]
public sealed class WebApiClientTemplate :
    GeneratedAttributeTemplateBase<
        ServiceContractAttribute,
        WebApiClientTemplate.SourceModel>
{
    protected override string Id =>
        "ServiceContract";

    protected override string AttrName =>
        "BD.Common8.Ipc.Attributes.ServiceContractAttribute";

    protected override string FileId => "WebApiClient";

    protected override ServiceContractAttribute GetAttribute(ImmutableArray<AttributeData> attributes)
    {
        return null!;
    }

    /// <summary>
    /// 从源码中读取并分析生成器所需要的模型
    /// </summary>
    public readonly record struct SourceModel : ISourceModel
    {
        /// <inheritdoc cref="INamedTypeSymbol"/>
        public required INamedTypeSymbol NamedTypeSymbol { get; init; }

        /// <summary>
        /// 命名空间
        /// </summary>
        public required string Namespace { get; init; }

        /// <summary>
        /// 类型名
        /// </summary>
        public required string TypeName { get; init; }

        /// <summary>
        /// 方法，函数
        /// </summary>
        public required ImmutableArray<IMethodSymbol> Methods { get; init; }

        /// <inheritdoc cref="ServiceContractAttribute"/>
        public required ServiceContractAttribute Attribute { get; init; }
    }

    protected override SourceModel GetSourceModel(GetSourceModelArgs args)
    {
        var methods = args.symbol.GetMembers().OfType<IMethodSymbol>().ToImmutableArray();

        SourceModel model = new()
        {
            NamedTypeSymbol = args.symbol,
            Namespace = args.@namespace,
            TypeName = args.typeName,
            Attribute = args.attr,
            Methods = methods,
        };
        return model;
    }

    protected override void WriteFile(Stream stream, SourceModel m)
    {
        if (m.Methods.Length == 0) return;

        WriteFileHeader(stream);
        stream.WriteNewLine();
        WriteNamespace(stream, m.Namespace);
        stream.WriteNewLine();
        stream.WriteFormat(
"""
public class {0}Impl(
    ILoggerFactory loggerFactory,
    IClientHttpClientFactory clientFactory) :
    WebApiClientBaseService(
        loggerFactory.CreateLogger(TAG),
        clientFactory,
        null),
    {1}
"""u8, m.TypeName.TrimStart('I'), m.TypeName);
        stream.WriteNewLine();
        stream.WriteCurlyBracketLeft();
        stream.WriteNewLine();
        stream.WriteFormat(
"""
    const string TAG = "{0}";
"""u8, m.TypeName.TrimStart('I').TrimEnd("Service"));
        stream.WriteNewLine();
        stream.WriteNewLine();
        stream.Write(
"""
    protected override string ClientName => TAG;
"""u8);
        stream.WriteNewLine();
        stream.WriteNewLine();

        foreach (var method in m.Methods)
        {
            var category = method.GetMethodParametersCategory();
            switch (category)
            {
                case MethodParametersCategory.None:
                case MethodParametersCategory.SimpleTypes:
                case MethodParametersCategory.FromBody:
                    break;
                default:
                    continue;
            }

            var returnType = method.ReturnType.ToDisplayString();
            const string taskMarkPrefix = "System.Threading.Tasks.Task<";
            if (returnType.StartsWith(taskMarkPrefix))
            {
                var len = returnType.Length - taskMarkPrefix.Length - 1;
                returnType = returnType.Substring(taskMarkPrefix.Length, len);
            }
            const string apiRspMarkPrefix = "BD.Common8.Primitives.ApiRsp.Models.ApiRspImpl<";
            if (returnType.StartsWith(apiRspMarkPrefix))
            {
                var len = returnType.Length - apiRspMarkPrefix.Length - 1;
                returnType = returnType.Substring(apiRspMarkPrefix.Length, len);
            }
            var isApiRspImplByReturnType = returnType == "BD.Common8.Primitives.ApiRsp.Models.ApiRspImpl";

            if (isApiRspImplByReturnType)
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
                for (int i = 0; i < method.Parameters.Length; i++)
                {
                    var parameter = method.Parameters[i];
                    var typeString = category.GetParameterTypeString(parameter);
                    if (i == 0)
                    {
                        stream.WriteFormat(
"""
{0} {1}
"""u8, typeString, parameter.Name);
                    }
                    else
                    {
                        stream.WriteFormat(
"""
, {0} {1}
"""u8, typeString, parameter.Name);
                    }
                }
            }

            stream.WriteNewLine();
            stream.Write(
"""
    {
"""u8);
            stream.WriteNewLine();

            stream.Write(
"""
        var client = CreateClient();
"""u8);
            stream.WriteNewLine();

            IParameterSymbol parameter;
            switch (category)
            {
                case MethodParametersCategory.None:
                    stream.WriteFormat(
"""
        const string requestUri = "/{0}";
        using var rsp = await client.PostAsync(requestUri, null);
"""u8, method.Name);
                    break;
                case MethodParametersCategory.SimpleTypes:
                    stream.WriteFormat(
"""
        string requestUri = $"/{0}
"""u8, method.Name);
                    for (int i = 0; i < method.Parameters.Length; i++)
                    {
                        parameter = method.Parameters[i];
                        stream.Write(
"""
/{
"""u8);
                        stream.WriteUtf16StrToUtf8OrCustom(parameter.Name);
                        stream.Write(
"""
}
"""u8);
                    }
                    stream.Write(
"""
";
"""u8);
                    stream.WriteNewLine();
                    stream.WriteFormat(
"""
        using var rsp = await client.PostAsync(requestUri, null);
"""u8, method.Name);
                    break;
                case MethodParametersCategory.FromBody:
                    parameter = method.Parameters[0];
                    stream.WriteFormat(
"""
        const string requestUri = "/{0}";
        using var content = GetHttpContent({1});
        using var rsp = await client.PostAsync(requestUri, content);
"""u8, method.Name, parameter.Name);
                    break;
            }
            stream.WriteNewLine();
            if (isApiRspImplByReturnType)
            {
                stream.Write(
"""
        var r = await ReadFromAsync<ApiRspImpl>(rsp.Content);
"""u8);
            }
            else
            {
                stream.WriteFormat(
"""
        var r = await ReadFromAsync<ApiRspImpl<{0}>>(rsp.Content);
"""u8, returnType);
            }
            stream.WriteNewLine();
            stream.Write(
"""
        return r!;
"""u8);

            stream.WriteNewLine();
            stream.Write(
"""
    }
"""u8);
            stream.WriteNewLine();
            stream.WriteNewLine();
        }

        stream.WriteCurlyBracketRight();
    }
}
