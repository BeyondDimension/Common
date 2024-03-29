namespace BD.Common8.SourceGenerator.Bcl.Templates;

[Generator]
public sealed class ViewModelWrapperTemplate :
    GeneratedAttributeTemplateBase<
        ViewModelWrapperTemplate.AttributeModel,
        ViewModelWrapperTemplate.SourceModel>
{
    protected override string Id =>
        "ViewModelWrapper";

    protected override string AttrName =>
        "System.Runtime.CompilerServices.ViewModelWrapperGeneratedAttribute";

    public readonly record struct AttributeModel
    {
        /// <inheritdoc cref="ViewModelWrapperGeneratedAttribute.Properties"/>
        public Dictionary<string, Type>? DictProperties { get; init; }

        /// <inheritdoc cref="ViewModelWrapperGeneratedAttribute"/>
        public required ViewModelWrapperGeneratedAttribute Attribute { get; init; }

        /// <summary>
        /// 类型是否支持 MemoryPack
        /// </summary>
        public required bool IsMemoryPack { get; init; }
    }

    protected override AttributeModel GetAttribute(ImmutableArray<AttributeData> attributes)
    {
        var attribute = attributes.FirstOrDefault(x => x.ClassNameEquals(AttrName));
        attribute.ThrowIsNull();

        var isMemoryPack = attributes.Any(x => x.ClassNameEquals("MemoryPack.MemoryPackableAttribute"));

        if (attribute.ConstructorArguments.FirstOrDefault().GetObjectValue()
            is not INamedTypeSymbol modelType)
            throw new ArgumentOutOfRangeException(nameof(modelType));

        Dictionary<string, Type>? dictProperties = null;
        ViewModelWrapperGeneratedAttribute attr = new(new TypeStringImpl(modelType));
        foreach (var item in attribute.ThrowIsNull().NamedArguments)
        {
            var value = item.Value.GetObjectValue();
            switch (item.Key)
            {
                case nameof(ViewModelWrapperGeneratedAttribute.Constructor):
                    attr.Constructor = Convert.ToBoolean(value);
                    break;
                case nameof(ViewModelWrapperGeneratedAttribute.ImplicitOperator):
                    attr.ImplicitOperator = Convert.ToBoolean(value);
                    break;
                case nameof(ViewModelWrapperGeneratedAttribute.ImplicitOperatorNotNull):
                    attr.ImplicitOperatorNotNull = Convert.ToBoolean(value);
                    break;
                case nameof(ViewModelWrapperGeneratedAttribute.IsSealed):
                    attr.IsSealed = Convert.ToBoolean(value);
                    break;
                case nameof(ViewModelWrapperGeneratedAttribute.ViewModelBaseType):
                    attr.ViewModelBaseType = TypeStringImpl.Parse(value?.ToString());
                    break;
                case nameof(ViewModelWrapperGeneratedAttribute.Properties):
                    var properties = ((IEnumerable<object>?)value)?.OfType<string>().ToArray();
                    if (properties != null)
                    {
                        attr.Properties = properties;
                        var modelProperties = modelType.GetMembers().OfType<IPropertySymbol>();
                        foreach (var p in properties)
                        {
                            var modelProperty = modelProperties.FirstOrDefault(x => x.Name == p);
                            dictProperties ??= [];
                            dictProperties.Add(p, TypeStringImpl.Parse(modelProperty.Type));
                        }
                    }
                    break;
            }
        }
        return new AttributeModel
        {
            Attribute = attr,
            DictProperties = dictProperties,
            IsMemoryPack = isMemoryPack,
        };
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

        /// <inheritdoc cref="AttributeModel"/>
        public required AttributeModel AttrModel { get; init; }

        /// <inheritdoc cref="ViewModelWrapperGeneratedAttribute"/>
        public ViewModelWrapperGeneratedAttribute Attribute => AttrModel.Attribute;

        /// <inheritdoc/>
        AttributeModel ISourceModel.Attribute => AttrModel;
    }

    protected override SourceModel GetSourceModel(GetSourceModelArgs args)
    {
        SourceModel model = new()
        {
            NamedTypeSymbol = args.symbol,
            Namespace = args.@namespace,
            TypeName = args.typeName,
            AttrModel = args.attr,
        };
        return model;
    }

    static bool UseEqualityComparer(Type type)
    {
        return false;
        //var typeSymbol = TypeStringImpl.GetTypeSymbol(type);
        //if (typeSymbol == null)
        //    return true;

        ////typeSymbol.TypeKind == TypeKind.Class

        //if (string.Equals("string", typeSymbol.Name, StringComparison.OrdinalIgnoreCase))
        //    return false;

        //if (typeSymbol.IsSimpleTypes())
        //    return false;

        //var methods = typeSymbol.GetMembers().OfType<IMethodSymbol>();

        //const string methodName = "op_Inequality";
        //var has_op_Inequality = methods
        //    .Any(static x => x.Name == methodName);
        //if (has_op_Inequality) // 是否实现了不相等运算符
        //    return false;

        //return true;
    }

    protected override void WriteFile(Stream stream, SourceModel m)
    {
        var modelTypeSymbol = TypeStringImpl.GetTypeSymbol(m.AttrModel.Attribute.ModelType);
        modelTypeSymbol.ThrowIsNull();

        var modelAttrs = modelTypeSymbol.GetAttributes();
        //var isMP2 = modelAttrs.Any(static x => x.ClassNameEquals("MemoryPack.MemoryPackableAttribute"));
        bool isMP2 = false; // TODO

        var debuggerDisplayProperty = modelTypeSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(x => x.DeclaredAccessibility == Accessibility.Public &&
                x.Name == "DebuggerDisplay" && TypeStringImpl.Parse(x.Type).IsSystemString).FirstOrDefault();
        var debuggerDisplayMethod = debuggerDisplayProperty == null ? modelTypeSymbol.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(x => x.DeclaredAccessibility == Accessibility.Public &&
                (x.Name == "DebuggerDisplay" || x.Name == "GetDebuggerDisplay") && TypeStringImpl.Parse(x.ReturnType).IsSystemString).FirstOrDefault() : null;

        WriteFileHeader(stream);
        stream.WriteNewLine();
        WriteNamespace(stream, m.Namespace);
        stream.WriteNewLine();
        var vmBaseType = m.Attribute.ViewModelBaseType?.Name ?? (m.AttrModel.IsMemoryPack ? "ReactiveSerializationObject" : "ReactiveObject");
        var modelType = m.Attribute.ModelType?.Name;
        modelType.ThrowIsNull();

        stream.WriteFormat(
"""
/// <summary>
/// <see cref="{0}"/> 的视图模型
/// </summary>

"""u8, modelType);

        if (debuggerDisplayProperty != null)
        {
            stream.Write(
"""
[DebuggerDisplay("{DebuggerDisplay,nq}")]

"""u8);
        }
        else if (debuggerDisplayMethod != null)
        {
            stream.Write(
"""
[DebuggerDisplay("{
"""u8);
            stream.WriteUtf16StrToUtf8OrCustom(debuggerDisplayMethod.Name);
            stream.Write(
"""
(),nq}")]

"""u8);
        }
        if (m.Attribute.Constructor)
        {
            stream.WriteFormat(m.Attribute.IsSealed ?
"""
sealed partial class {0}({2} model) : {1}
"""u8
                :
"""
partial class {0}({2} model) : {1}
"""u8, m.TypeName, vmBaseType, modelType);
        }
        else
        {
            stream.WriteFormat(m.Attribute.IsSealed ?
"""
sealed partial class {0} : {1}
"""u8
                :
"""
partial class {0} : {1}
"""u8, m.TypeName, vmBaseType);
        }
        if (isMP2)
        {
            stream.WriteFormat(
"""
, IMemoryPackable<{0}>, IFixedSizeMemoryPackable
"""u8, m.TypeName);
        }
        stream.WriteNewLine();
        stream.WriteCurlyBracketLeft(); // {
        stream.WriteNewLine();

        #region Body

        if (m.AttrModel.IsMemoryPack)
        {
            stream.WriteFormat(
"""
    /// <summary>
    /// Initializes a new instance of the <see cref="{0}"/> class.
"""u8, m.TypeName);
            stream.WriteFormat(
"""

    /// </summary>
    [MP2Constructor, SystemTextJsonConstructor]
    public {0}() : this(new())

"""u8, m.TypeName);
            stream.Write(
"""
    {
    }


"""u8);
        }

        if (debuggerDisplayProperty != null)
        {
            stream.Write(
"""
    /// <inheritdoc cref="DebuggerDisplayAttribute"/>
    [XmlIgnore, IgnoreDataMember, SystemTextJsonIgnore, NewtonsoftJsonIgnore, MPIgnore, MP2Ignore]
    public string DebuggerDisplay => Model.DebuggerDisplay!;


"""u8);
        }
        else if (debuggerDisplayMethod != null)
        {
            stream.WriteFormat(
"""
    /// <inheritdoc cref="DebuggerDisplayAttribute"/>
    public string {0}() => Model.{0}()!;


"""u8, debuggerDisplayMethod.Name);
        }

        var anyModel = m.AttrModel.IsMemoryPack || m.NamedTypeSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Any(x => x.Name == "Model");

        if (m.Attribute.Constructor)
        {
            if (!anyModel)
                stream.WriteFormat(
"""
    /// <inheritdoc cref="{0}"/>
    public {0} Model { get; } = model;
"""u8, modelType);
        }
        else
        {
            if (!anyModel)
                stream.WriteFormat(
"""
    /// <inheritdoc cref="{0}"/>
    public {0} Model { get; }
"""u8, modelType);
        }
        stream.WriteNewLine();

        if (m.Attribute.ImplicitOperator)
        {
            stream.WriteNewLine();
            stream.WriteFormat(
"""
    /// <summary>
    /// ViewModel => Model
    /// </summary>
    /// <param name="vm"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator {0}({1} vm) => vm.Model;

    /// <summary>
    /// Model => ViewModel
    /// </summary>
    /// <param name="model"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
"""u8, modelType, m.TypeName);
            stream.WriteNewLine();
            if (m.Attribute.ImplicitOperatorNotNull)
            {
                stream.WriteFormat(
"""
    public static implicit operator {0}({1} model) => new(model);
"""u8, m.TypeName, modelType);
            }
            else
            {
                stream.WriteFormat(
"""
    public static implicit operator {0}?([NotNullIfNotNull(nameof(model))] {1}? model) => model is null ? null : new(model);
"""u8, m.TypeName, modelType);
            }
            stream.WriteNewLine();
        }

        static ImmutableArray<IPropertySymbol> GetVMMProperties(ImmutableArray<ISymbol> symbols)
        {
            var properties = symbols.OfType<IPropertySymbol>();
            properties = properties.Where(x
                => x.DeclaredAccessibility == Accessibility.Public);
            return properties.ToImmutableArray();
        }

        var vmProperties = GetVMMProperties(m.NamedTypeSymbol.GetMembers()); // 视图模型的属性
        var mProperties = GetVMMProperties(modelTypeSymbol.GetMembers()); // 模型的属性

        stream.WriteNewLine();
        foreach (var property in mProperties)
        {
            if (property.IsGeneratorProperty())
                continue; // 如果模型类为 record 则会生成该属性，跳过

            Type? propertyType = null;
            var isReactiveProperty = m.AttrModel.DictProperties != null && m.AttrModel.DictProperties.TryGetValue(property.Name, out propertyType);
            propertyType ??= TypeStringImpl.Parse(property.Type);

            var p = new KeyValuePair<string, Type>(property.Name, propertyType);
            if (isReactiveProperty)
            {
                // 如果有手动属性，例如需要标注验证的特性，则生成 get/set 函数，否则将生成属性
                // 参考 https://learn.microsoft.com/zh-cn/dotnet/communitytoolkit/mvvm/generators/observableproperty#requesting-property-validation
                if (vmProperties.Any(x => x.Name == p.Key))
                {
                    stream.WriteFormat(
"""
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    {1} _{0}() => Model.{0};

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void _{0}({1} value)
"""u8, p.Key, p.Value);
                    stream.Write(
"""

    {

"""u8);
                    if (UseEqualityComparer(p.Value))
                    {
                        stream.WriteFormat(
"""
        if (!EqualityComparer<{0}>.Default.Equals(Model.{1}, value))
"""u8, p.Value, p.Key);
                    }
                    else
                    {
                        stream.WriteFormat(
"""
        if (Model.{0} != value)
"""u8, p.Key);
                    }
                    stream.Write(
"""

        {
"""u8);
                    stream.WriteNewLine();
                    stream.WriteFormat(
"""
            this.RaisePropertyChanging(nameof({0}));
"""u8, p.Key); // 生成的函数需要手动传递 propertyName
                    stream.WriteNewLine();
                    stream.WriteFormat(
"""
            Model.{0} = value;
"""u8, p.Key);
                    stream.WriteNewLine();
                    stream.WriteFormat(
"""
            this.RaisePropertyChanged(nameof({0}));
"""u8, p.Key); // 生成的函数需要手动传递 propertyName
                    stream.WriteNewLine();
                    stream.Write(
"""
        }
    }

"""u8);
                }
                else
                {
                    stream.WriteFormat(
"""
    /// <inheritdoc cref="{0}.{1}"/>
    [XmlIgnore, IgnoreDataMember, SystemTextJsonIgnore, NewtonsoftJsonIgnore, MPIgnore, MP2Ignore]
    public {2} {1}
"""u8, modelType, p.Key, p.Value);
                    stream.Write(
"""

    {

"""u8);
                    stream.WriteFormat(
"""
        get => Model.{0};
"""u8, p.Key);
                    stream.Write(
"""

        set
        {

"""u8);
                    if (UseEqualityComparer(p.Value))
                    {
                        stream.WriteFormat(
"""
            if (!EqualityComparer<{0}>.Default.Equals(Model.{1}, value))
"""u8, p.Value, p.Key);
                    }
                    else
                    {
                        stream.WriteFormat(
"""
            if (Model.{0} != value)
"""u8, p.Key);
                    }
                    stream.Write(
"""

            {
                this.RaisePropertyChanging();

"""u8);
                    stream.WriteFormat(
"""
                Model.{0} = value;
"""u8, p.Key);
                    stream.Write(
"""

                this.RaisePropertyChanged();
            }
        }
    }

"""u8);
                }
            }
            else
            {
                if (vmProperties.Any(x => x.Name == p.Key))
                    continue; // 视图模型中手动定义了该属性则跳过生成

                stream.WriteFormat(
"""
    /// <inheritdoc cref="{0}.{1}"/>
    [XmlIgnore, IgnoreDataMember, SystemTextJsonIgnore, NewtonsoftJsonIgnore, MPIgnore, MP2Ignore]
    public {2} {1} 
"""u8, modelType, p.Key, p.Value);
                stream.Write("{"u8);
                var hasSetMethod = property.SetMethod != null;
                var hasGetMethod = property.GetMethod != null;
                if (hasSetMethod && hasGetMethod)
                {
                    stream.WriteFormat(
"""
 get => Model.{1}; set => Model.{1} = value; 
"""u8, modelType, p.Key, p.Value);
                }
                else if (!hasSetMethod)
                {
                    stream.WriteFormat(
"""
 get => Model.{1}; 
"""u8, modelType, p.Key, p.Value);
                }
                else
                {
                    stream.WriteFormat(
"""
 set => Model.{1} = value; 
"""u8, modelType, p.Key, p.Value);
                }
                stream.Write("}"u8);
                stream.WriteNewLine();
            }
            stream.WriteNewLine();
        }

        // if 重写 ToString
        var methods = modelTypeSymbol.GetMembers().OfType<IMethodSymbol>();

        var isOverrideToString = methods.Any(static x => x.IsObjectToString());
        if (isOverrideToString) // 如果模型类重写了 ToString，那么视图模型也要重写
        {
            if (!m.NamedTypeSymbol.GetMembers().OfType<IMethodSymbol>().Any(static x => x.IsObjectToString())) // 视图模型没有手写的重写 ToString
            {
                stream.Write(
"""

    /// <inheritdoc />
    public override string ToString() => Model.ToString()!;
"""u8);
            }
        }

        if (isMP2)
        {
            stream.WriteNewLine();
            stream.Write(
"""
    static SizePositionModel()
    {
        global::MemoryPack.MemoryPackFormatterProvider.Register<SizePositionModel>();
    }

    [global::MemoryPack.Internal.Preserve]
    static int global::MemoryPack.IFixedSizeMemoryPackable.Size => 

    [global::MemoryPack.Internal.Preserve]
    static void IMemoryPackFormatterRegister.RegisterFormatter()
    {
        if (!global::MemoryPack.MemoryPackFormatterProvider.IsRegistered<SizePositionModel>())
        {
            global::MemoryPack.MemoryPackFormatterProvider.Register(new global::MemoryPack.Formatters.MemoryPackableFormatter<SizePositionModel>());
        }
        if (!global::MemoryPack.MemoryPackFormatterProvider.IsRegistered<SizePositionModel[]>())
        {
            global::MemoryPack.MemoryPackFormatterProvider.Register(new global::MemoryPack.Formatters.ArrayFormatter<SizePositionModel>());
        }
    }

    [global::MemoryPack.Internal.Preserve]
    static void IMemoryPackable<SizePositionModel>.Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer, scoped ref SizePositionModel? value)
    {
    }

    [global::MemoryPack.Internal.Preserve]
    static void IMemoryPackable<SizePositionModel>.Deserialize(ref MemoryPackReader reader, scoped ref SizePositionModel? value)
    {
    }
"""u8);
        }

        // IMemoryPackable<T>, IFixedSizeMemoryPackable

        #endregion

        stream.WriteNewLine();
        stream.WriteCurlyBracketRight(); // }
        stream.WriteNewLine();
    }
}
