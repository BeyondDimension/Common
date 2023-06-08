namespace BD.Common.Repositories.SourceGenerator.Models;

/// <summary>
/// 属性元数据
/// </summary>
/// <param name="Field"></param>
/// <param name="PropertyType"></param>
/// <param name="Name"></param>
/// <param name="HumanizeName"></param>
/// <param name="FixedProperty"></param>
public record struct PropertyMetadata(
    IFieldSymbol Field,
    string PropertyType,
    string Name,
    string HumanizeName,
    FixedProperty FixedProperty)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly ReadOnlySpan<byte> GetBaseInterfaceType() => FixedProperty switch
    {
        FixedProperty.Id => "Entity<PropertyType>"u8,
        FixedProperty.TenantId => "Tenant"u8,
        FixedProperty.CreateUserId => "CreateUserIdNullable"u8,
        _ => (ReadOnlySpan<byte>)Encoding.UTF8.GetBytes(FixedProperty.ToString()),
    };

    /// <summary>
    /// 写入继承的接口类型
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="isFirstWrite"></param>
    public readonly void WriteBaseInterfaceType(Stream stream, ref bool isFirstWrite)
    {
        // : IsFirst
        // , IsNotFirst
        if (Enum.IsDefined(typeof(FixedProperty), FixedProperty))
        {
            if (isFirstWrite)
            {
                stream.Write(
"""
 : 
"""u8);
                stream.WriteByte(I);
                stream.Write(GetBaseInterfaceType());
                isFirstWrite = false;
            }
            else
            {
                stream.Write(
"""
, 
"""u8);
                stream.WriteByte(I);
                stream.Write(GetBaseInterfaceType());
            }
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

        FixedPropertyHelper.Analysis(
            field,
            ref propertyType,
            out var fieldName,
            out var fieldHumanizeName,
            out var fixedProperty);
        return new(field, propertyType, fieldName, fieldHumanizeName, fixedProperty);
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
    public static void WriteRequired(Stream stream)
    {
        stream.Write(
"""
    [Required]

"""u8);
    }

    /// <summary>
    /// 将属性元数据写入到源码文件流中
    /// </summary>
    /// <param name="stream"></param>
    public readonly void Write(Stream stream)
    {
        var attributes = Field.GetAttributes();

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
                new(ClassType.Entities, stream, attribute, attributeClassFullName, this));
            if (writeAttribute != null)
                writeAttributes.Add(writeAttribute);
            //foreach (var handle in attributeHandles.Value)
            //{
            //    var writeAttribute = handle.Write(new(stream, attribute, attributeClassFullName, this));
            //    if (writeAttribute != null)
            //        writeAttributes.Add(writeAttribute);
            //}
        }

        if (!writeAttributes.Contains(TypeFullNames.Comment))
        {
            // 属性缺少 Comment 自动补全
            WriteComment(stream, HumanizeName);
        }

        var constantValue = Field.IsConst ? Field.ConstantValue : null;
        var propertyType = PropertyType;

        #region String 类型特殊处理

        switch (propertyType)
        {
            case "string": // 类型为 String 不可 null 的
                constantValue = "\"\""; // 需要设置默认值空字符串
                if (!writeAttributes.Contains(TypeFullNames.Required))
                {
                    // 并且数据库必填
                    WriteRequired(stream);
                }
                break;
            case "string?": // 类型为 String 可为 null 的
                if (writeAttributes.Contains(TypeFullNames.Required)) // 但是有数据库必填
                {
                    // 将类型更改为不可 null，并设置默认值空字符串
                    propertyType = "string";
                    constantValue = "\"\"";
                }
                break;
        }

        #endregion

        var property =
"""
    public {0} {1} { get; set; }
"""u8;
        stream.WriteFormat(property, propertyType, Name);
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
    /// <param name="stream"></param>
    /// <param name="i"></param>
    /// <param name="length"></param>
    public readonly void Write(Stream stream, int i, int length)
    {
        Write(stream);
        if (i < length - 1)
        {
            stream.Write(
"""


"""u8);
        }
    }
}
