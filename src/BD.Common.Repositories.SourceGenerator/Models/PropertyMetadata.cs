namespace BD.Common.Repositories.SourceGenerator.Models;

/// <summary>
/// 属性元数据
/// </summary>
public record struct PropertyMetadata(
    IFieldSymbol Field,
    ImmutableArray<AttributeData> Attributes,
    string PropertyType,
    string Name,
    string HumanizeName,
    FixedProperty FixedProperty,
    BackManageFieldAttribute? BackManageField)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly ReadOnlySpan<byte> GetBaseInterfaceType(ClassType classType) => FixedProperty switch
    {
        FixedProperty.Id => classType switch
        {
            ClassType.Entities => Encoding.UTF8.GetBytes($"Entity<{PropertyType}>"),
            _ => Encoding.UTF8.GetBytes($"KeyModel<{PropertyType}>"),
        },
        FixedProperty.TenantId => "Tenant"u8,
        FixedProperty.CreateUserId => "CreateUserIdNullable"u8,
        FixedProperty.IPAddress => "IPAddress"u8,
        _ => Encoding.UTF8.GetBytes(FixedProperty.ToString()),
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly byte[]? GetBaseEntityType(EntityBaseClassType baseClassType) => baseClassType switch
    {
        EntityBaseClassType.Entity => Encoding.UTF8.GetBytes($"Entity<{PropertyType}>"),
        EntityBaseClassType.TenantBaseEntityV2 => Encoding.UTF8.GetBytes($"TenantBaseEntityV2<{PropertyType}>"),
        EntityBaseClassType.OperatorBaseEntityV2 => Encoding.UTF8.GetBytes($"OperatorBaseEntityV2<{PropertyType}>"),
        EntityBaseClassType.CreationBaseEntityV2 => Encoding.UTF8.GetBytes($"CreationBaseEntityV2<{PropertyType}>"),
        _ => default,
    };

    public static void WriteBaseType(Stream stream, ReadOnlySpan<byte> classType, ref bool isFirstWrite, bool writeI = true)
    {
        // : IsFirst
        // , IsNotFirst
        if (isFirstWrite)
        {
            stream.Write(
"""
 : 
"""u8);
            if (writeI)
                stream.WriteByte(I);
            stream.Write(classType);
            isFirstWrite = false;
        }
        else
        {
            stream.Write(
"""
, 
"""u8);
            if (writeI)
                stream.WriteByte(I);
            stream.Write(classType);
        }
    }

    /// <summary>
    /// 写入继承的接口类型
    /// </summary>
    public readonly void WriteBaseInterfaceType(Stream stream, ClassType classType, ref bool isFirstWrite)
    {
        if (Enum.IsDefined(typeof(FixedProperty), FixedProperty))
        {
            var classType_ = GetBaseInterfaceType(classType);
            WriteBaseType(stream, classType_, ref isFirstWrite, true);
        }
    }

    /// <summary>
    /// 从 <see cref="IFieldSymbol"/> 中读取属性元数据
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyMetadata Parse(IFieldSymbol field)
    {
        // 从 IFieldSymbol 中解析需要的内容，通过此模型类去生成源码，解析仅执行一次
        var propertyType = field.Type.ToDisplayString();
        var indexOf = propertyType.LastIndexOf('.');
        if (indexOf >= 0)
        {
            propertyType = propertyType[(indexOf + 1)..];
        }

        var attributes = field.GetAttributes();
        var backManageField = attributes.GetBackManageFieldAttribute();
        FixedPropertyHelper.Analysis(
            field,
            ref propertyType,
            out var fieldName,
            out var fieldHumanizeName,
            out var fixedProperty);
        return new(field, attributes, propertyType,
            fieldName, fieldHumanizeName, fixedProperty,
            backManageField);
    }

    /// <summary>
    /// 从多个 <see cref="IFieldSymbol"/> 中读取属性元数据
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImmutableArray<PropertyMetadata> Parse(ImmutableArray<IFieldSymbol> fields)
        => fields.Select(Parse).ToImmutableArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteComment(Stream stream, string humanizeName)
    {
        stream.WriteFormat(
"""
    [Comment("{0}")]

"""u8, humanizeName);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteAttribute(Stream stream, ReadOnlySpan<byte> attribute)
    {
        stream.WriteFormat(
"""
    [{0}]

"""u8, attribute.ToArray());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteRequired(Stream stream)
    {
        stream.Write(
"""
    [Required]

"""u8);
    }

    public readonly void WriteParam(Stream stream)
    {
        var format =
"""
/// <param name="{0}">{1}</param>
"""u8;
        var propertyName = GeneratorConfig.Translate(Name);
        stream.WriteFormat(format, propertyName, HumanizeName);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    /// <summary>
    /// 将属性元数据写入到源码文件流中
    /// </summary>
    public readonly void Write(Stream stream, ClassType classType, bool @override = false)
    {
        var attributes = Attributes;
        foreach (var handle in propertyHandles.Value)
        {
            if (handle.Write(new(stream, attributes, this)))
            {
                return;
            }
        }

        var summary =
"""
    /// <summary>
    /// {0}
    /// </summary>

"""u8;
        stream.WriteFormat(summary, HumanizeName);

        HashSet<string> writeAttributes = new();
        foreach (var attribute in attributes)
        {
            var attributeClassFullName = attribute.GetClassFullName();
            if (attributeClassFullName == null)
                continue;
            var writeAttribute = GeneralAttributeHandle.Instance.Write(
                new(classType, stream, attribute, attributeClassFullName, this));
            if (writeAttribute != null)
                writeAttributes.Add(writeAttribute);
            //foreach (var handle in attributeHandles.Value)
            //{
            //    var writeAttribute = handle.Write(new(stream, attribute, attributeClassFullName, this));
            //    if (writeAttribute != null)
            //        writeAttributes.Add(writeAttribute);
            //}
        }

        switch (classType)
        {
            case ClassType.Entities:
                if (!writeAttributes.Contains(TypeFullNames.Comment))
                {
                    // 属性缺少 Comment 自动补全
                    WriteComment(stream, HumanizeName);
                }
                switch (FixedProperty)
                {
                    case FixedProperty.Id:
                        if (!writeAttributes.Contains(TypeFullNames.Key))
                        {
                            // 主键 Id 属性缺少 Key 自动补全
                            WriteAttribute(stream, "Key"u8);
                        }
                        break;
                }
                break;
        }

        var constantValue = Field.IsConst ? Field.ConstantValue : null;
        var propertyType = PropertyType;

        #region String 类型特殊处理

        switch (propertyType)
        {
            case "string": // 类型为 String 【不可】 null 的
                constantValue = "\"\""; // 需要设置默认值空字符串
                if (!writeAttributes.Contains(TypeFullNames.Required))
                {
                    // 并且数据库必填
                    WriteRequired(stream);
                }
                break;
            case "string?": // 类型为 String 【可】为 null 的
                if (writeAttributes.Contains(TypeFullNames.Required)) // 但是有数据库必填
                {
                    // 将类型更改为不可 null，并设置默认值空字符串
                    propertyType = "string";
                    constantValue = "\"\"";
                }
                break;
        }

        #endregion

        var property = @override ?
"""
    public override {0} {1} { get; set; }
"""u8 :
"""
    public {0} {1} { get; set; }
"""u8;
        var propertyName = GeneratorConfig.Translate(Name);
        stream.WriteFormat(property, propertyType, propertyName);
        if (constantValue != null)
        {
            var propertyConstantValue =
"""
 = {0};
"""u8;
            stream.WriteFormat(propertyConstantValue, constantValue);
        }
        stream.Write(
"""


"""u8);
    }

    /// <summary>
    /// 将属性元数据写入到源码文件流中，并根据下标与长度判断是否为最后一个不添加额外的空行
    /// </summary>
    public readonly void Write(Stream stream, ClassType classType, int i, int length, bool @override = false)
    {
        Write(stream, classType, @override);
        if (i < length - 1)
        {
            stream.Write(
"""


"""u8);
        }
    }
}
