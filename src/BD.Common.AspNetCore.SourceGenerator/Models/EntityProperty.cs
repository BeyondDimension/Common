namespace BD.Common.SourceGenerator.Models;

/// <summary>
/// 表实体属性
/// </summary>
/// <param name="TypeName">类型名称</param>
/// <param name="PropertyName">属性名称</param>
/// <param name="DefaultValue">默认值</param>
/// <param name="Required">是否不能为 null</param>
/// <param name="Summary">属性的注释</param>
/// <param name="RelationshipTypeName">关联的其他表实体类型</param>
/// <param name="RelationshipPropertyName">关联的其他表实体属性名称</param>
/// <param name="MaxLength">最大长度</param>
/// <param name="MinLength">最小长度</param>
/// <param name="Precision">浮点型精度 PrecisionAttribute(Int32)</param>
/// <param name="PrecisionScale">浮点型小数位数 PrecisionAttribute(Int32, Int32)</param>
/// <param name="IsBMQuery">该属性是否为后台管理系统中表格查询</param>
/// <param name="IsBMAdd">该属性是否为后台管理系统中可新增时填写</param>
/// <param name="IsBMEdit">该属性是否为后台管理系统中可编辑时修改</param>
/// <param name="IsIndex">该属性是否为索引</param>
public sealed record class EntityProperty(
    string TypeName = "",
    string PropertyName = "",
    string? DefaultValue = default,
    bool Required = false,
    string? Summary = default,
    string? RelationshipTypeName = default,
    string? RelationshipPropertyName = default,
    int? MaxLength = default,
    string? MaxLengthConstName = default,
    int? MinLength = default,
    int? Precision = default,
    int? PrecisionScale = default,
    bool IsBMQuery = default,
    bool IsBMAdd = default,
    bool IsBMEdit = default,
    bool IsIndex = default)
{
    [Obsolete]
    public bool IsAvatar()
    {
        return TypeName == "Guid?" && PropertyName.OICEquals("Avatar");
    }

    public bool IsCreationTime()
    {
        return ((TypeName == "DateTimeOffset" || TypeName == "DateTime") && PropertyName.OICEquals("CreationTime"))
            || Summary == "创建时间";
    }

    public bool IsDescribe()
    {
        return ((TypeName == "string?" || TypeName == "string") && (PropertyName.OICEquals("Describe") || PropertyName.OICEquals("Description")))
            || Summary == "描述";
    }

    public bool IsDisable()
    {
        return (TypeName == "bool" && PropertyName.OICEquals("Disable"))
            || Summary == "是否禁用该条"
            || Summary == "是否禁用";
    }

    public bool IsDisableReason()
    {
        return ((TypeName == "string?" || TypeName == "string") && (PropertyName.OICEquals("DisableReason") || PropertyName.OICEquals("ReasonForDisabling") || PropertyName.OICEquals("ReasonDisabling") || PropertyName.OICEquals("ReasonDisable")))
            || Summary == "禁用原因";
    }

    public bool IsGender()
    {
        return ((TypeName == "Gender" || TypeName == "Gender?") && PropertyName.OICEquals("Gender"))
            || Summary == "性别";
    }

    public bool IsNickName()
    {
        return ((TypeName == "string?" || TypeName == "string") && PropertyName.OICEquals("NickName"))
            || Summary == "昵称";
    }

    public bool IsOrder()
    {
        return (TypeName == "long" && (PropertyName.OICEquals("Order") || PropertyName.OICEquals("Sort")))
            || Summary == "排序";
    }

    public bool IsIsTop()
    {
        return (TypeName == "bool" && PropertyName.OICEquals("IsTop"))
            || Summary == "是否置顶"
            || Summary == "置顶";
    }

    public bool IsIPAddress()
    {
        return ((TypeName == "string?" || TypeName == "string") && PropertyName.OICEquals("IPAddress"))
            || Summary.OICEquals("IP 地址")
            || Summary.OICEquals("IP地址");
    }

    public bool IsPassword()
    {
        return ((TypeName == "string" || TypeName == "string?") && (PropertyName.OICEquals("Password") || PropertyName.OICEquals("Pwd")))
            || Summary == "密码";
    }

    public bool IsRemarks()
    {
        return ((TypeName == "string" || TypeName == "string?") && (PropertyName.OICEquals("Remarks") || PropertyName.OICEquals("Remark") || PropertyName.OICEquals("Notes") || PropertyName.OICEquals("Note")))
            || Summary == "备注";
    }

    public bool IsSmsCode()
    {
        return ((TypeName == "string" || TypeName == "string?") && (PropertyName.OICEquals("SmsCode") || PropertyName.OICEquals("Sms")))
            || Summary == "短信验证码";
    }

    public bool IsSoftDeleted()
    {
        return (TypeName == "bool" && (PropertyName.OICEquals("SoftDeleted") || PropertyName.OICEquals("IsDeleted") || PropertyName.OICEquals("IsDelete")))
            || Summary == "软删除"
            || Summary == "是否软删除";
    }

    public bool IsTenantId()
    {
        return (TypeName == "Guid" && PropertyName.OICEquals("TenantId"))
            || Summary.OICEquals("租户 Id")
            || Summary.OICEquals("租户Id");
    }

    public bool IsTitle()
    {
        return ((TypeName == "string" || TypeName == "string?") && PropertyName.OICEquals("Title"))
            || Summary.OICEquals("标题");
    }

    public bool IsUpdateTime()
    {
        return ((TypeName == "DateTimeOffset" || TypeName == "DateTime") && PropertyName.OICEquals("UpdateTime"))
            || Summary == "更新时间"
            || Summary == "编辑时间"
            || Summary == "修改时间";
    }

    public bool IsCreateUserId()
    {
        return ((TypeName.OICEquals("Guid") || TypeName.OICEquals("Guid?")) && PropertyName.OICEquals("CreateUserId"))
            || Summary == "创建人"
            || Summary == "创建人(创建此条目的后台管理员)"
            || Summary == "创建人（创建此条目的后台管理员）";
    }

    public bool IsOperatorUserId()
    {
        return (TypeName.OICEquals("Guid?") && PropertyName.OICEquals("OperatorUserId"))
            || Summary == "操作人"
            || Summary == "编辑人"
            || Summary == "修改人"
            || Summary == "最后一次操作的人(纪录后台管理员禁用或启用或编辑该条的操作)"
            || Summary == "最后一次操作的人(记录后台管理员禁用或启用或编辑该条的操作)"
            || Summary == "最后一次操作的人（纪录后台管理员禁用或启用或编辑该条的操作）"
            || Summary == "最后一次操作的人（记录后台管理员禁用或启用或编辑该条的操作）";
    }
}
