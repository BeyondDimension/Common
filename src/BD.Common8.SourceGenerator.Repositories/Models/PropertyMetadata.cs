namespace BD.Common8.SourceGenerator.Repositories.Models;

/// <summary>
/// 属性元数据
/// </summary>
public record struct PropertyMetadata(
    EntityDesignPropertyMetadata Field,
    string PropertyType,
    string Name,
    string HumanizeName,
    FixedProperty FixedProperty,
    BackManageFieldAttribute? BackManageField)
{
    public string CamelizeName { get; set; } = null!;

    public string TranslateName { get; set; } = null!;

    public void Calculate()
    {
        TranslateName ??= GeneratorConfig.Translate(Name);
        CamelizeName ??= TranslateName.Camelize();
    }

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
        EntityBaseClassType.TenantBaseEntity => Encoding.UTF8.GetBytes($"TenantBaseEntity<{PropertyType}>"),
        EntityBaseClassType.OperatorBaseEntity => Encoding.UTF8.GetBytes($"OperatorBaseEntity<{PropertyType}>"),
        EntityBaseClassType.CreationBaseEntity => Encoding.UTF8.GetBytes($"CreationBaseEntity<{PropertyType}>"),
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
    /// 从 <see cref="EntityDesignPropertyMetadata"/> 中读取属性元数据
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyMetadata Parse(KeyValuePair<string, EntityDesignPropertyMetadata> field)
    {
        var propertyType = field.Value.TypeName!;
        var backManageField = field.Value.Attribute;
        FixedPropertyHelper.Analysis(
            field.Value.Name!,
            ref propertyType,
            out var fieldName,
            out var fieldHumanizeName,
            out var fixedProperty);

        PropertyMetadata metadata = new(field.Value, propertyType,
            fieldName, fieldHumanizeName, fixedProperty,
            backManageField);
        metadata.Calculate();
        return metadata;
    }

    /// <summary>
    /// 从多个 <see cref="EntityDesignPropertyMetadata"/> 中读取属性元数据
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImmutableArray<PropertyMetadata> Parse(Dictionary<string, EntityDesignPropertyMetadata> fields)
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

    public readonly void WriteParam(Stream stream, string name, string content)
    {
        var format =
"""

    /// <param name="{0}">{1}</param>
"""u8;
        stream.WriteFormat(format, name, content);
    }

    /// <summary>
    /// 将属性元数据写入到源码文件流中
    /// </summary>
    public readonly void Write(Stream stream, ClassType classType, bool @override = false)
    {
        PropertyHandles.Value.First().Write(new(stream, Field.PreprocessorDirective, true, this));

        var humanizeName = HumanizeName;

        switch (FixedProperty)
        {
            case FixedProperty.CreateUserId:
                humanizeName = $"创建人 UserId（创建此条目的后台管理员）";
                break;
            case FixedProperty.OperatorUserId:
                humanizeName = $"最后一次操作的人 UserId（记录后台管理员禁用或启用或编辑该条的操作）";
                break;
        }

        if (Field.Summary != null)
        {
            var summary =
"""
{0}

"""u8;
            stream.WriteFormat(summary, Regex.Replace(Field.Summary, @"[ ]*///", "    ///"));
        }
        else if (FixedProperty != default)
        {
            var summary =
"""
    /// <inheritdoc/>

"""u8;
            stream.Write(summary);
        }
        else
        {
            var summary =
"""
    /// <summary>
    /// {0}
    /// </summary>

"""u8;
            stream.WriteFormat(summary, humanizeName);
        }

        HashSet<string> writeAttributes = new();

        var properties = typeof(EntityDesignPropertyMetadata).GetProperties();
        foreach (var pinfo in properties)
        {
            if (new[] { "Name", "TypeName", "DefaultValue", "Attribute", "Summary", "PreprocessorDirective", "Modifier", "IsValueType" }
                .Any(x => pinfo.Name.Contains(x)))
                continue;

            var attrvalue = pinfo.GetValue(Field);

            if (attrvalue == null)
                continue;

            var writeAttribute = GeneralAttributeHandle.Instance.Write(
           new(classType, stream, pinfo.Name, attrvalue.ToString(), this));

            if (writeAttribute != null)
                writeAttributes.Add(writeAttribute);
        }

        switch (classType)
        {
            case ClassType.Entities:
                if (!writeAttributes.Contains(TypeFullNames.Comment))
                    // 属性缺少 Comment 自动补全
                    WriteComment(stream, HumanizeName);
                switch (FixedProperty)
                {
                    case FixedProperty.Id:
                        if (!writeAttributes.Contains(TypeFullNames.Key))
                            // 主键 Id 属性缺少 Key 自动补全
                            WriteAttribute(stream, "Key"u8);
                        break;
                }
                break;
        }

        //var constantValue = Field.IsConst ? Field.ConstantValue : null;
        var constantValue = Field.DefaultValue;
        var propertyType = PropertyType;

        #region String 类型特殊处理
        if (constantValue == null)
        {
            switch (propertyType)
            {
                case "string": // 类型为 String 【不可】 null 的
                    Field.Modifier = "required";
                    if (!writeAttributes.Contains(TypeFullNames.Required))
                        // 并且数据库必填
                        WriteRequired(stream);
                    break;
                case "string?": // 类型为 String 【可】为 null 的
                    if (writeAttributes.Contains(TypeFullNames.Required)) // 但是有数据库必填
                    {
                        // 将类型更改为不可 null，并设置默认值空字符串
                        propertyType = "string";
                        Field.Modifier = "required";
                    }
                    break;
            }
        }
        #endregion

        if (writeAttributes.Contains(TypeFullNames.Required))
        {
            Field.Modifier = "required";
        }

        var property = @override ?
"""
    public override {0} {1} { get; set; }
"""u8 :
"""
    public {0} {1} { get; set; }
"""u8;

        var propertyName = GeneratorConfig.Translate(Name);
        if (Field.Modifier != null)
        {
            property =
"""
    public {0} {1} {2} { get; set; }
"""u8;
            stream.WriteFormat(property, Field.Modifier, propertyType, propertyName);
        }
        else
        {
            stream.WriteFormat(property, propertyType, propertyName);
        }

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

        switch (classType)
        {
            case ClassType.BackManageTableModels:
                switch (FixedProperty)
                {
                    case FixedProperty.CreateUserId:
                        stream.Write(
"""

    /// <summary>
    /// 创建人（创建此条目的后台管理员）
    /// </summary>
    public string? CreateUser { get; set; }

"""u8);
                        break;
                    case FixedProperty.OperatorUserId:
                        stream.Write(
"""

    /// <summary>
    /// 最后一次操作的人（记录后台管理员禁用或启用或编辑该条的操作）
    /// </summary>
    public string? OperatorUser { get; set; }

"""u8);
                        break;
                }
                break;
        }

        PropertyHandles.Value.First().Write(new(stream, Field.PreprocessorDirective, false, this));
    }

    /// <summary>
    /// 将属性元数据写入到源码文件流中，并根据下标与长度判断是否为最后一个不添加额外的空行
    /// </summary>
    public readonly void Write(Stream stream, ClassType classType, int i, int length, bool @override = false)
    {
        Write(stream, classType, @override);
        if (i < length - 1)
            stream.Write(
"""


"""u8);
    }
}