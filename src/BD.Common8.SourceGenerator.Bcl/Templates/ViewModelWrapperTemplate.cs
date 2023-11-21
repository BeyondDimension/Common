namespace BD.Common8.SourceGenerator.Bcl.Templates;

#pragma warning disable SA1600 // Elements should be documented

sealed class ViewModelWrapperTemplate : TemplateBase
{
    const string Id = "ViewModelWrapper";

    public const string AttrName =
        $"System.Runtime.CompilerServices.{Id}GeneratedAttribute";

    internal readonly record struct AttributeModel
    {
        /// <inheritdoc cref="ViewModelWrapperGeneratedAttribute.Properties"/>
        public Dictionary<string, Type>? DictProperties { get; init; }

        /// <inheritdoc cref="ViewModelWrapperGeneratedAttribute"/>
        public required ViewModelWrapperGeneratedAttribute Attribute { get; init; }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static AttributeModel GetAttribute(ImmutableArray<AttributeData> attributes)
    {
        var attribute = attributes.FirstOrDefault(static x => x.ClassNameEquals(AttrName));
        attribute.ThrowIsNull();

        if (attribute.ConstructorArguments.FirstOrDefault().GetObjectValue()
            is not INamedTypeSymbol modelType)
            throw new ArgumentOutOfRangeException(nameof(modelType));

        Dictionary<string, Type>? dictProperties = null;
        ViewModelWrapperGeneratedAttribute attr = new(new TypeStringImpl(modelType.ToString()));
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
        };
    }

    /// <summary>
    /// 从源码中读取并分析生成器所需要的模型
    /// </summary>
    internal readonly record struct SourceModel
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
    }

    public static void Execute(SourceProductionContext spc, GeneratorAttributeSyntaxContext m)
    {
        if (m.TargetSymbol is not INamedTypeSymbol symbol)
            return;

        var @namespace = symbol.ContainingNamespace.ToDisplayString();
        var typeName = symbol.Name;

        var attr = GetAttribute(symbol.GetAttributes());

        SourceModel model = new()
        {
            NamedTypeSymbol = symbol,
            Namespace = @namespace,
            TypeName = typeName,
            AttrModel = attr,
        };
        Execute(spc, model);
    }

    /// <summary>
    /// 源生成器执行逻辑
    /// </summary>
    /// <param name="spc"></param>
    /// <param name="m"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Execute(SourceProductionContext spc, SourceModel m)
    {
        SourceText sourceText;
        try
        {
            using var memoryStream = new MemoryStream();
            WriteFile(memoryStream, m);
            sourceText = SourceText.From(memoryStream, canBeEmbedded: true);
#if DEBUG
            var sourceTextString = sourceText.ToString();
            Console.WriteLine();
            Console.WriteLine(sourceTextString);
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
            sourceText = builder.ToSourceText();
        }
        spc.AddSource($"{m.Namespace}.{m.TypeName}.{Id}.g.cs", sourceText);
    }

    static bool UseEqualityComparer(Type type)
    {
        var typeSymbol = TypeStringImpl.GetTypeSymbol(type);
        if (typeSymbol == null)
            return true;

        if (string.Equals("string", typeSymbol.Name, StringComparison.OrdinalIgnoreCase))
            return false;

        if (typeSymbol.IsSimpleTypes())
            return false;

        return true;
    }

    /// <summary>
    /// 写入源码
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="m"></param>
    public static void WriteFile(
        Stream stream,
        SourceModel m)
    {
        try
        {
            WriteFileHeader(stream);
            stream.WriteNewLine();
            WriteNamespace(stream, m.Namespace);
            stream.WriteNewLine();
            var vmBaseType = m.Attribute.ViewModelBaseType?.Name ?? "ReactiveObject";
            var modelType = m.Attribute.ModelType?.Name;
            modelType.ThrowIsNull();
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
            stream.WriteNewLine();
            stream.WriteCurlyBracketLeft(); // {
            stream.WriteNewLine();

            #region Body

            if (m.Attribute.Constructor)
            {
                stream.WriteFormat(
"""
    /// <inheritdoc cref="{0}"/>
    public {0} Model { get; } = model;
"""u8, modelType);
            }
            else
            {
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
                stream.Write(
"""
    /// <summary>
    /// Model => ViewModel
    /// </summary>
    /// <param name="model"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
"""u8);
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

            if (m.AttrModel.DictProperties != null)
            {
                var modelProperties = m.NamedTypeSymbol.GetMembers().OfType<IPropertySymbol>().ToImmutableArray();
                foreach (var p in m.AttrModel.DictProperties)
                {
                    stream.WriteNewLine();

                    // 如果有手动属性，例如需要标注验证的特性，则生成 get/set 函数，否则将生成属性
                    // 参考 https://learn.microsoft.com/zh-cn/dotnet/communitytoolkit/mvvm/generators/observableproperty#requesting-property-validation
                    if (modelProperties.Any(x => x.Name == p.Key))
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
            }

            #endregion

            stream.WriteCurlyBracketRight(); // }
            stream.WriteNewLine();
        }
        catch (OperationCanceledException)
        {
        }
    }
}
