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

    /// <inheritdoc/>
    protected override ServiceContractImplAttribute GetAttribute(ImmutableArray<AttributeData> attributes)
    {
        var attribute = attributes.FirstOrDefault(x => x.ClassNameEquals(AttrName));
        attribute.ThrowIsNull();

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
            return null!;
        }

        return new(serviceType, generatorType);
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

        var methods = serviceType.GetMembers().OfType<IMethodSymbol>()
            .Where(static m => !m.IsStatic)
            .ToImmutableArray();

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
}
