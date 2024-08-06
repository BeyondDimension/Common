namespace BD.Common8.SourceGenerator.Ipc.Templates.Abstractions;

/// <summary>
/// 用于 Ipc 的源文件基类模板
/// </summary>
public abstract class IpcTemplateBase :
    GeneratedAttributeTemplateBase<
        ServiceContractImplAttribute,
        IpcTemplateBase.SourceModel>
{
    /// <inheritdoc/>
    protected sealed override string Id =>
        "ServiceContractImpl";

    /// <inheritdoc/>
    protected sealed override string AttrName =>
        "BD.Common8.Ipc.Attributes.ServiceContractImplAttribute";

    protected override IEnumerable<ServiceContractImplAttribute> GetMultipleAttributes(ImmutableArray<AttributeData> attributes)
    {
        var items = attributes.Where(x => x.ClassNameEquals(AttrName));
        foreach (var attribute in items)
        {
            Type? serviceType = null;
            IpcGeneratorType generatorType = default;
            for (int i = 0; i < attribute.ConstructorArguments.Length; i++)
            {
                var value = attribute.ConstructorArguments[i].GetObjectValue();
                switch (i)
                {
                    case 0:
                        serviceType = value is ITypeSymbol typeSymbolValue ?
                            TypeStringImpl.Parse(typeSymbolValue) : null;
                        break;
                    case 1:
                        generatorType = (IpcGeneratorType)Enum.Parse(typeof(IpcGeneratorType),
                            value?.ToString());
                        break;
                }
            }

            if (serviceType == null)
            {
                IgnoreExecute = true;
                break;
            }

            ServiceContractImplAttribute attr = new(serviceType, generatorType);

            foreach (var item in attribute.NamedArguments)
            {
                var value = item.Value.GetObjectValue();
                switch (item.Key)
                {
                    case nameof(ServiceContractImplAttribute.HubUrl):
                        attr.HubUrl = value?.ToString();
                        break;
                    case nameof(ServiceContractImplAttribute.HubTypeFullName):
                        attr.HubTypeFullName = value?.ToString()!;
                        if (string.IsNullOrWhiteSpace(attr.HubTypeFullName))
                            attr.HubTypeFullName = ServiceContractImplAttribute.DefaultHubTypeFullName;
                        break;
                }
            }

            yield return attr;
        }
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

        /// <inheritdoc cref="ServiceContractImplAttribute"/>
        public required ServiceContractImplAttribute Attribute { get; init; }
    }

    /// <inheritdoc/>
    protected sealed override SourceModel GetSourceModel(GetSourceModelArgs args)
    {
        var serviceType = TypeStringImpl.GetTypeSymbol(args.attr.ServiceType);
        serviceType.ThrowIsNull();

        var methods = serviceType.GetMembers().OfType<IMethodSymbol>().Where(static m => !m.IsStatic).ToImmutableArray();
        methods = methods.AddRange(serviceType.AllInterfaces.SelectMany(s => s.GetMembers().OfType<IMethodSymbol>()
            .Where(static m => !m.IsStatic)
            ).ToImmutableArray());

        if (methods.Length == 0)
        {
            IgnoreExecute = true;
            return default;
        }

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

    /// <summary>
    /// 根据 <see cref="IMethodSymbol"/> 获取返回值类型
    /// </summary>
    /// <param name="method"></param>
    /// <param name="isApiRspImplByReturnType"></param>
    /// <param name="isAsyncEnumerableByReturnType"></param>
    /// <returns></returns>
    protected TypeStringImpl GetReturnType(IMethodSymbol method,
        out bool isApiRspImplByReturnType,
        out bool isAsyncEnumerableByReturnType)
    {
        ITypeSymbol returnTypeSymbol = method.ReturnType;
        var returnType = new TypeStringImpl(returnTypeSymbol);
        const string taskMarkPrefix = "System.Threading.Tasks.Task<";
        if (returnType.FullName.StartsWith(taskMarkPrefix))
        {
            var len = returnType.FullName.Length - taskMarkPrefix.Length - 1;
            returnType = new TypeStringImpl(returnType.FullName.Substring(taskMarkPrefix.Length, len));
        }
        const string apiRspMarkPrefix = "BD.Common8.Models.ApiRspImpl<";
        if (returnType.FullName.StartsWith(apiRspMarkPrefix))
        {
            var len = returnType.FullName.Length - apiRspMarkPrefix.Length - 1;
            returnType = new TypeStringImpl(returnType.FullName.Substring(apiRspMarkPrefix.Length, len));
        }
        isApiRspImplByReturnType = returnType.FullName == "BD.Common8.Models.ApiRspImpl";
        isAsyncEnumerableByReturnType = returnType.IsSystemCollectionsGenericIAsyncEnumerable;
        return returnType;
    }

    /// <summary>
    /// 方法参数模型类
    /// </summary>
    /// <param name="ParaType"></param>
    /// <param name="ParaName"></param>
    /// <param name="ParaNameWithDefaultValue"></param>
    protected record struct MethodPara(
        TypeStringImpl ParaType,
        string ParaName,
        string ParaNameWithDefaultValue);

    /// <summary>
    /// 根据 <see cref="IMethodSymbol"/> 获取方法参数
    /// </summary>
    /// <param name="method"></param>
    /// <param name="category"></param>
    /// <returns></returns>
    protected MethodPara[] GetMethodParas(IMethodSymbol method,
        out MethodParametersCategory category)
    {
        category = method.GetMethodParametersCategory();
        var methodParas = new MethodPara[method.Parameters.Length];
        for (int i = 0; i < methodParas.Length; i++)
        {
            var parameter = method.Parameters[i];
            var paraTypeString = category.GetParameterTypeString(parameter);
            var paraName = parameter.Name;
            var paraType = new TypeStringImpl(paraTypeString)
            {
                TypeSymbol = parameter.Type,
            };
            var paraNameWithDefaultValue = paraName;
            switch (paraTypeString)
            {
                case "CancellationToken":
                    if (i == method.Parameters.Length - 1)
                        paraNameWithDefaultValue = $"{paraName} = default";
                    break;
            }
            methodParas[i] = new(paraType, paraName, paraNameWithDefaultValue);
        }
        return methodParas;
    }

    /// <summary>
    /// 根据方法参数返回请求方法
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    protected string GetRequestMethod(MethodParametersCategory category)
    {
        var requestMethod = category switch
        {
            MethodParametersCategory.FromBody or
            MethodParametersCategory.GeneratorModelFromBody => nameof(HttpMethod.Post),
            _ => nameof(HttpMethod.Get),
        };
        return requestMethod;
    }

    /// <summary>
    /// 写入 <see cref="Tuple"/>
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="methodParas"></param>
    protected void WriteTuple(Stream stream, MethodPara[] methodParas)
    {
        int endLen = 0;
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
                stream.Write(", "u8);
            }
            if (i % 7 == 0)
            {
                stream.Write("Tuple<"u8);
                endLen++;
            }
            stream.WriteUtf16StrToUtf8OrCustom(paraType.ToString());
        }
        for (int i = 0; i < endLen; i++)
        {
            stream.Write(">"u8);
        }
    }
}
