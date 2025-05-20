using BD.Common8.Ipc.Attributes;
using BD.Common8.Ipc.Enums;
using BD.Common8.SourceGenerator.Helpers;
using BD.Common8.SourceGenerator.Ipc.Enums;
using BD.Common8.SourceGenerator.Ipc.Templates.Abstractions;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace BD.Common8.SourceGenerator.Ipc.Templates;

/// <summary>
/// 用于 Ipc 服务端调用的源文件模板
/// </summary>
[Generator]
public sealed class IpcServerTemplate : IpcTemplateBase
{
    /// <inheritdoc/>
    protected override string FileId => "IpcServer";

    protected override IEnumerable<ServiceContractImplAttribute> GetMultipleAttributes(ImmutableArray<AttributeData> attributes)
    {
        var items = base.GetMultipleAttributes(attributes);
        foreach (var attribute in items)
        {
            var isBreak = false;
            switch (attribute.GeneratorType)
            {
                case IpcGeneratorType.Server:
                    break;
                default:
                    isBreak = true; // 非服务端生成类型直接跳过
                    break;
            }
            if (!isBreak)
                yield return attribute;
        }
    }

    static void WriteUsings(Stream stream)
    {
        stream.Write(
"""
using global::Microsoft.AspNetCore.Builder;


"""u8);
    }

    /// <inheritdoc/>
    protected override void WriteFile(Stream stream, in SourceModel m)
    {
        WriteFileHeader(stream);
        stream.Write(
"""
#pragma warning disable IDE0004 // 删除不必要的强制转换
#pragma warning disable CS8619 // 值中的引用类型的为 Null 性与目标类型不匹配。

"""u8);
        stream.WriteNewLine();
        WriteUsings(stream);
        WriteNamespace(stream, m.Namespace, isFileNamespace: false);
        if (!string.IsNullOrWhiteSpace(m.Namespace))
        {
            stream.WriteCurlyBracketLeft();
            stream.WriteNewLine();
        }
        // https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/minimal-apis
        stream.WriteFormat(
"""
partial class {0} : global::BD.Common8.Ipc.Services.IEndpointRouteMapGroup
"""u8, m.TypeName);
        stream.WriteNewLine();
        stream.WriteCurlyBracketLeft();
        stream.WriteNewLine();
        stream.Write(
"""
    /// <inheritdoc cref="global::BD.Common8.Ipc.Services.IEndpointRouteMapGroup.OnMapGroup(IEndpointRouteBuilder)"/>
    [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    static void global::BD.Common8.Ipc.Services.IEndpointRouteMapGroup.OnMapGroup(global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder endpoints)
    {

"""u8);
        stream.Write(
"""
        var builder = endpoints.MapGroup("
"""u8);
        WriteServiceTypeAndMethodName(stream, m.Attribute.ServiceType, null!, isMap: true);
        stream.Write(
"""
").RequireAuthorization();
"""u8);
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
", (Delegate)(static (global::Microsoft.AspNetCore.Http.HttpContext ctx
"""u8);
            }
            else
            {
                stream.Write(
"""
", (Delegate)(static async (global::Microsoft.AspNetCore.Http.HttpContext ctx
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
, [global::Microsoft.AspNetCore.Mvc.FromRoute] {0} {1}
"""u8, paraType, paraName);
                        }
                    }
                    break;
                case MethodParametersCategory.FromBody:
                    {
                        var (paraType, paraName, _) = methodParas[0];
                        var type = TypeStringImpl.GetTypeSymbol(paraType);
                        var isStruct = type?.TypeKind == TypeKind.Struct;

                        if (isStruct || type?.NullableAnnotation == NullableAnnotation.Annotated)
                        {
                            stream.WriteFormat(
"""
, [global::Microsoft.AspNetCore.Mvc.FromBody] {0} {1}
"""u8, paraType, paraName);
                        }
                        else
                        {
                            stream.WriteFormat(
"""
, [global::Microsoft.AspNetCore.Mvc.FromBody] {0}? {1}
"""u8, paraType, paraName);
                        }
                    }
                    break;
                case MethodParametersCategory.GeneratorModelFromBody:
                    {
                        stream.Write(
"""
, [global::Microsoft.AspNetCore.Mvc.FromBody] 
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
) => global::System.Ioc.Get<{0}>().{1}(
"""u8, m.Attribute.ServiceType, method.Name);
            }
            else
            {
                stream.Write(
"""
) =>
        {

"""u8);
                if (isApiRspImplByReturnType)
                {
                    stream.Write(
"""
            global::BD.Common8.Models.ApiRspImpl result;

"""u8);
                }
                else
                {
                    stream.WriteFormat(
"""
            global::BD.Common8.Models.ApiRspImpl<{0}> result;

"""u8, returnType);
                }
                stream.Write(
"""
            try
            {

"""u8);
                stream.WriteFormat(
"""
                result = await global::System.Ioc.Get<{0}>().{1}(
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
            if (!isAsyncEnumerableByReturnType)
            {
                stream.Write(
"""
ctx.RequestAborted);
            }
            catch (Exception ex)
            {
                result = ex;
            }
            return result;
        }));

"""u8);
            }
            else
            {
                stream.Write(
"""
ctx.RequestAborted)));

"""u8);
            }
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

        (string hubTypeNamespace, string hubTypeName) = GetHubTypeInfo(m.Attribute.HubTypeFullName);
        static (string hubTypeNamespace, string hubTypeName) GetHubTypeInfo(string hubTypeFullName)
        {
            hubTypeFullName = hubTypeFullName.TrimEnd('.');
            var lastIndexOfHubTypeFullNameD = hubTypeFullName.LastIndexOf('.');
            if (lastIndexOfHubTypeFullNameD != -1)
            {
                var hubTypeNamespace = hubTypeFullName[..lastIndexOfHubTypeFullNameD];
                var hubTypeName = hubTypeFullName[(lastIndexOfHubTypeFullNameD + 1)..];
                return (hubTypeNamespace, hubTypeName);
            }

            return (hubTypeFullName, hubTypeFullName);
        }

        WriteNamespace(stream, hubTypeNamespace, isFileNamespace: false, isFirstWriteNamespace: false);
        stream.WriteCurlyBracketLeft();
        stream.WriteNewLine();
        //        stream.WriteFormat(
        //"""
        //[Authorize]
        //file sealed class {0} : global::Microsoft.AspNetCore.SignalR.Hub
        //"""u8, hubTypeName);
        //        stream.WriteFormat(
        //"""
        //sealed partial class {0} : global::Microsoft.AspNetCore.SignalR.Hub
        //"""u8, hubTypeName);
        // https://learn.microsoft.com/zh-cn/aspnet/core/signalr/hubs
        stream.WriteFormat(
"""
partial class {0} : global::Microsoft.AspNetCore.SignalR.Hub
"""u8, hubTypeName);
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

            if (isAsyncEnumerableByReturnType)
            {
                stream.WriteFormat(
"""
    public global::System.Collections.Generic.IAsyncEnumerable<{0}> 
"""u8, returnType.GenericT);
                WriteServiceTypeAndMethodName(stream, m.Attribute.ServiceType, method, separatorIs_: true, isString: false);
                stream.Write("("u8);
                WriteParameters(m);
                stream.Write(
"""
)
"""u8);
            }
            else if (isApiRspImplByReturnType)
            {
                stream.Write(
"""
    public async Task<global::BD.Common8.Models.ApiRspImpl> 
"""u8);
                WriteServiceTypeAndMethodName(stream, m.Attribute.ServiceType, method, separatorIs_: true, isString: false);
                stream.Write("("u8);
                WriteParameters(m);
                stream.Write(
"""
)
"""u8);
            }
            else
            {
                stream.WriteFormat(
"""
    public async Task<global::BD.Common8.Models.ApiRspImpl<{0}>> 
"""u8, returnType);
                WriteServiceTypeAndMethodName(stream, m.Attribute.ServiceType, method, separatorIs_: true, isString: false);
                stream.Write("("u8);

                WriteParameters(m);
                stream.Write(
"""
)
"""u8);
            }

            void WriteParameters(in SourceModel m)
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

            void WriteMethodBody(in SourceModel m)
            {
                if (isAsyncEnumerableByReturnType)
                {
                    stream.WriteFormat(
"""
        var result = global::System.Ioc.Get<{0}>().
"""u8, m.Attribute.ServiceType);
                }
                else
                {
                    if (isApiRspImplByReturnType)
                    {
                        stream.Write(
"""
        global::BD.Common8.Models.ApiRspImpl result;

"""u8);
                    }
                    else
                    {
                        stream.WriteFormat(
"""
        global::BD.Common8.Models.ApiRspImpl<{0}> result;

"""u8, returnType);
                    }
                    stream.Write(
"""
        try
        {

"""u8);
                    stream.WriteFormat(
"""
            result = await global::System.Ioc.Get<{0}>().
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

                if (isAsyncEnumerableByReturnType)
                {
                    stream.Write(
"""
global::BD.Common8.Ipc.Extensions.HubExtensions.RequestAborted(this));
        return result!;

"""u8);
                }
                else
                {
                    stream.Write(
"""
global::BD.Common8.Ipc.Extensions.HubExtensions.RequestAborted(this));
        }
        catch (Exception ex)
        {
            result = ex;
        }
        return result;

"""u8);
                }
            }

            WriteMethodBody(m);

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
