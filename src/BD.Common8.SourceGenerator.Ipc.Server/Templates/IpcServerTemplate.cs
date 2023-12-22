namespace BD.Common8.SourceGenerator.Ipc.Templates;

/// <summary>
/// 用于 Ipc 服务端调用的源文件模板
/// </summary>
[Generator]
public sealed class IpcServerTemplate : IpcTemplateBase
{
    /// <inheritdoc/>
    protected override string FileId => "IpcServer";

    /// <inheritdoc/>
    protected override ServiceContractImplAttribute GetAttribute(ImmutableArray<AttributeData> attributes)
    {
        var attr = base.GetAttribute(attributes);
        switch (attr.GeneratorType)
        {
            case IpcGeneratorType.Server:
                break;
            default:
                IgnoreExecute = true; // 非服务端生成类型直接跳过
                return null!;
        }
        return attr;
    }

    /// <inheritdoc/>
    protected override void WriteFile(Stream stream, SourceModel m)
    {
        WriteFileHeader(stream);
        stream.Write(
"""
#pragma warning disable IDE0004 // 删除不必要的强制转换

"""u8);
        stream.WriteNewLine();
        WriteNamespace(stream, m.Namespace, isFileNamespace: false);
        if (!string.IsNullOrWhiteSpace(m.Namespace))
        {
            stream.WriteCurlyBracketLeft();
            stream.WriteNewLine();
        }
        stream.WriteFormat(
"""
partial class {0} : IEndpointRouteMapGroup
"""u8, m.TypeName);
        stream.WriteNewLine();
        stream.WriteCurlyBracketLeft();
        stream.WriteNewLine();
        stream.Write(
"""
    /// <inheritdoc cref="IEndpointRouteMapGroup.OnMapGroup(IEndpointRouteBuilder)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void IEndpointRouteMapGroup.OnMapGroup(IEndpointRouteBuilder endpoints)
    {

"""u8);
        stream.WriteFormat(
"""
        var builder = endpoints.MapGroup("/{0}").RequireAuthorization();
"""u8, m.Attribute.ServiceType);
        stream.WriteNewLine();

        var methodDatas = m.Methods.ToDictionary(static k => k, method =>
        {
            var methodParas = GetMethodParas(method,
                out var category);

            var returnType = GetReturnType(method,
                out var isApiRspImplByReturnType,
                out var isAsyncEnumerableByReturnType);

            return (methodParas,
                category,
                returnType,
                isApiRspImplByReturnType,
                isAsyncEnumerableByReturnType);
        });

        foreach (var methodData in methodDatas)
        {
            var method = methodData.Key;
            (var methodParas,
                var category,
                var returnType,
                var isApiRspImplByReturnType,
                var isAsyncEnumerableByReturnType) = methodData.Value;

            var requestMethod = GetRequestMethod(category);
            stream.WriteFormat(
"""
        builder.Map{1}("/{0}
"""u8, method.Name, requestMethod);
            switch (category)
            {
                case MethodParametersCategory.SimpleTypes:
                    {
                        for (int i = 0; i < methodParas.Length; i++)
                        {
                            var (paraType, paraName, _) = methodParas[i];
                            if (i == methodParas.Length - 1)
                            {
                                if (paraType.IsSystemThreadingCancellationToken)
                                    break;
                            }
                            stream.Write(
"""
/{
"""u8);
                            stream.WriteUtf16StrToUtf8OrCustom(paraName);
                            stream.Write(
"""
}
"""u8);
                        }
                    }
                    break;
            }
            if (isAsyncEnumerableByReturnType)
            {
                stream.Write(
"""
", (Delegate)(static (HttpContext ctx
"""u8);
            }
            else
            {
                stream.Write(
"""
", (Delegate)(static async (HttpContext ctx
"""u8);
            }
            switch (category)
            {
                case MethodParametersCategory.SimpleTypes:
                    {
                        for (int i = 0; i < methodParas.Length; i++)
                        {
                            var (paraType, paraName, _) = methodParas[i];
                            if (i == methodParas.Length - 1)
                            {
                                if (paraType.IsSystemThreadingCancellationToken)
                                    break;
                            }
                            stream.WriteFormat(
"""
, [FromRoute] {0} {1}
"""u8, paraType, paraName);
                        }
                    }
                    break;
                case MethodParametersCategory.FromBody:
                    {
                        var (paraType, paraName, _) = methodParas[0];
                        stream.WriteFormat(
"""
, [FromBody] {0} {1}
"""u8, paraType, paraName);
                    }
                    break;
                case MethodParametersCategory.GeneratorModelFromBody:
                    {
                        stream.Write(
"""
, [FromBody] 
"""u8);
                        WriteTuple(stream, methodParas);
                        stream.Write(
"""
 body
"""u8);
                    }
                    break;
            }
            if (isAsyncEnumerableByReturnType)
            {
                stream.WriteFormat(
"""
) => Ioc.Get<{0}>().{1}(
"""u8, m.Attribute.ServiceType, method.Name);
            }
            else
            {
                stream.WriteFormat(
"""
) => await Ioc.Get<{0}>().{1}(
"""u8, m.Attribute.ServiceType, method.Name);
            }
            bool isFirstMapMethodArg = true;
            switch (category)
            {
                case MethodParametersCategory.SimpleTypes:
                    {
                        for (int i = 0; i < methodParas.Length; i++)
                        {
                            var (paraType, paraName, _) = methodParas[i];
                            if (i == methodParas.Length - 1)
                            {
                                if (paraType.IsSystemThreadingCancellationToken)
                                    break;
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
                            isFirstMapMethodArg = false;
                        }
                    }
                    break;
                case MethodParametersCategory.FromBody:
                    {
                        var (_, paraName, _) = methodParas[0];
                        stream.WriteUtf16StrToUtf8OrCustom(paraName);
                        isFirstMapMethodArg = false;
                    }
                    break;
                case MethodParametersCategory.GeneratorModelFromBody:
                    {
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

                            // body.Item1, body.Item2, body.Item3, body.Item4, body.Item5, body.Item6, body.Item7,
                            // body.Rest.Item1, body.Rest.Item2, body.Rest.Item3, body.Rest.Item4, body.Rest.Item5, body.Rest.Item6, body.Rest.Item7,
                            // body.Rest.Rest.Item1, body.Rest.Rest.Item2, body.Rest.Rest.Item3, body.Rest.Rest.Item4
                            if (i != 0)
                            {
                                stream.Write(", "u8);
                            }

                            stream.Write("body"u8);

                            var v = i / 7;
                            for (int j = 0; j < v; j++)
                            {
                                stream.Write(".Rest"u8);
                            }
                            stream.Write(".Item"u8);

                            stream.WriteUtf16StrToUtf8OrCustom((i - (v * 7) + 1).ToString());
                        }
                        isFirstMapMethodArg = false;
                    }
                    break;
                default:
                    break;
            }

            if (!isFirstMapMethodArg)
            {
                stream.Write(
"""
, 
"""u8);
            }
            stream.Write(
"""
ctx.RequestAborted)));

"""u8);
        }
        stream.Write(
"""
    }
"""u8);
        stream.WriteNewLine();
        //stream.WriteNewLine();
        //        stream.Write(
        //"""
        //    /// <inheritdoc cref="IHubEndpointRouteMapHub.OnMapHub(IpcServerService)"/>
        //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //    static void IHubEndpointRouteMapHub.OnMapHub(IpcServerService ipcServerService)
        //    {

        //"""u8);
        //        var hubTypeName = Encoding.UTF8.GetBytes(
        //            $"{m.TypeName}_{GenerateRandomString(Random.Next(24, 32))}_Hub");
        //        stream.WriteFormat(
        //"""
        //        ipcServerService.MapHub<{0}, {1}>("/Hubs/{0}");
        //"""u8, m.Attribute.ServiceType, hubTypeName);
        //        stream.Write(
        //"""

        //    }

        //"""u8);
        stream.WriteCurlyBracketRight();
        stream.WriteNewLine();
        if (!string.IsNullOrWhiteSpace(m.Namespace))
        {
            stream.WriteCurlyBracketRight();
            stream.WriteNewLine();
        }
        WriteNamespace(stream, "BD.Common8.SourceGenerator.Ipc.Server", isFileNamespace: false, isFirstWriteNamespace: false);
        stream.WriteCurlyBracketLeft();
        stream.WriteNewLine();
        //        stream.WriteFormat(
        //"""
        //[Authorize]
        //file sealed class {0} : Hub
        //"""u8, hubTypeName);
        //        stream.WriteFormat(
        //"""
        //sealed partial class {0} : Hub
        //"""u8, hubTypeName);
        stream.WriteFormat(
"""
partial class IpcHub : Hub
"""u8);
        stream.WriteNewLine();
        stream.WriteCurlyBracketLeft();
        stream.WriteNewLine();

        foreach (var methodData in methodDatas)
        {
            var method = methodData.Key;
            (var methodParas,
                var category,
                var returnType,
                var isApiRspImplByReturnType,
                var isAsyncEnumerableByReturnType) = methodData.Value;
            var hubMethodName = $"{m.Attribute.ServiceType}_{method.Name}";

            if (isAsyncEnumerableByReturnType)
            {
                stream.WriteFormat(
"""
    public IAsyncEnumerable<{0}> {1}(
"""u8, returnType.GenericT, hubMethodName);
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
"""u8, hubMethodName);
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
"""u8, returnType, hubMethodName);

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
                    if (i == methodParas.Length - 1 && paraType.IsSystemThreadingCancellationToken)
                        break; // Hub 方法最后一个参数不能有 CancellationToken
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

            void WriteMethodBody()
            {
                if (isAsyncEnumerableByReturnType)
                {
                    stream.WriteFormat(
"""
        var result = Ioc.Get<{0}>().
"""u8, m.Attribute.ServiceType);
                }
                else
                {
                    stream.WriteFormat(
"""
        var result = await Ioc.Get<{0}>().
"""u8, m.Attribute.ServiceType);
                }

                stream.WriteFormat(
"""
{0}(
"""u8, method.Name);

                bool isFirstMapMethodArg = true;
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
                    isFirstMapMethodArg = false;
                }

                if (!isFirstMapMethodArg)
                {
                    stream.Write(
"""
, 
"""u8);
                }

                stream.Write(
"""
this.RequestAborted());
        return result!;

"""u8);
            }

            WriteMethodBody();

            stream.Write(
"""
    }
"""u8);
            stream.WriteNewLine();
            stream.WriteNewLine();
        }

        stream.WriteCurlyBracketRight();
        stream.WriteNewLine();

        stream.WriteCurlyBracketRight();
        stream.WriteNewLine();
    }
}
