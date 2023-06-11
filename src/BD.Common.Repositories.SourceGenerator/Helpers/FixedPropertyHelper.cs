namespace BD.Common.Repositories.SourceGenerator.Helpers;

/// <summary>
/// 固定属性助手类
/// </summary>
static class FixedPropertyHelper
{
    /// <summary>
    /// 分析固定属性
    /// </summary>
    /// <param name="field"></param>
    /// <param name="propertyType"></param>
    /// <param name="fieldName"></param>
    /// <param name="fieldHumanizeName"></param>
    /// <param name="fixedProperty"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Analysis(
        IFieldSymbol field,
        ref string propertyType,
        out string fieldName,
        out string fieldHumanizeName,
        out FixedProperty fixedProperty)
    {
        fieldName = field.Name;

        if ("Id".OICEquals(fieldName))
        {
            fixedProperty = FixedProperty.Id;
            fieldHumanizeName = "主键";
            return;
        }
        if (propertyType.IsDateTimeOrWithOffset())
        {
            if (fieldName == "创建时间" ||
                fieldName.OICEquals("CreationTime"))
            {
                fixedProperty = FixedProperty.CreationTime;
                propertyType = nameof(DateTimeOffset);
                fieldName = "CreationTime";
                fieldHumanizeName = "创建时间";
                return;
            }
            else if (fieldName == "更新时间" ||
                fieldName == "编辑时间" ||
                fieldName == "修改时间" ||
                fieldName.OICEquals("UpdateTime") ||
                fieldName.OICEquals("RenewTime") ||
                fieldName.OICEquals("EditTime") ||
                fieldName.OICEquals("ModificationTime") ||
                fieldName.OICEquals("Modified") ||
                fieldName.OICEquals("ModifiedTime") ||
                fieldName.OICEquals("ModifyTime"))
            {
                fixedProperty = FixedProperty.UpdateTime;
                propertyType = nameof(DateTimeOffset);
                fieldName = "UpdateTime";
                fieldHumanizeName = "更新时间";
                return;
            }
        }
        else if (propertyType.IsStringOrWithNullable())
        {
            if (fieldName == "描述" ||
                fieldName.OICEquals("Describe") ||
                fieldName.OICEquals("Description"))
            {
                fixedProperty = FixedProperty.Describe;
                propertyType = "string?";
                fieldName = "Describe";
                fieldHumanizeName = "描述";
                return;
            }
            else if (fieldName == "禁用原因" ||
                fieldName.OICEquals("DisableReason") ||
                fieldName.OICEquals("ReasonForDisabling") ||
                fieldName.OICEquals("ReasonDisabling") ||
                fieldName.OICEquals("ReasonDisable"))
            {
                fixedProperty = FixedProperty.DisableReason;
                propertyType = "string?";
                fieldName = "DisableReason";
                fieldHumanizeName = "禁用原因";
                return;
            }
            else if (fieldName == "昵称" ||
                fieldName.OICEquals("NickName"))
            {
                fixedProperty = FixedProperty.NickName;
                propertyType = "string?";
                fieldName = "NickName";
                fieldHumanizeName = "昵称";
                return;
            }
            else if (fieldName == "IP地址" ||
                fieldName.OICEquals("IPAddress"))
            {
                fixedProperty = FixedProperty.IPAddress;
                propertyType = "string";
                fieldName = "IPAddress";
                fieldHumanizeName = "IP 地址";
                return;
            }
            else if (fieldName == "密码" ||
                fieldName.OICEquals("Password") ||
                fieldName.OICEquals("Pwd"))
            {
                fixedProperty = FixedProperty.Password;
                propertyType = "string";
                fieldName = "Password";
                fieldHumanizeName = "密码";
                return;
            }
            else if (fieldName == "备注" ||
                fieldName.OICEquals("Remarks") ||
                fieldName.OICEquals("Remark") ||
                fieldName.OICEquals("Notes") ||
                fieldName.OICEquals("Note"))
            {
                fixedProperty = FixedProperty.Remarks;
                propertyType = "string?";
                fieldName = "Remarks";
                fieldHumanizeName = "备注";
                return;
            }
            else if (fieldName == "短信验证码" ||
                fieldName.OICEquals("SmsCode") ||
                fieldName.OICEquals("Sms"))
            {
                fixedProperty = FixedProperty.SmsCode;
                propertyType = "string?";
                fieldName = "SmsCode";
                fieldHumanizeName = "短信验证码";
                return;
            }
            else if (fieldName == "标题" ||
                fieldName.OICEquals("Title"))
            {
                fixedProperty = FixedProperty.Title;
                propertyType = "string";
                fieldName = "Title";
                fieldHumanizeName = "标题";
                return;
            }
        }
        else if (propertyType.IsGenderOrWithNullable())
        {
            if (fieldName == "性别" ||
                fieldName.OICEquals("Gender") ||
                fieldName.OICEquals("Sex") ||
                fieldName.OICEquals("Sexuality") ||
                fieldName.OICEquals("Sexed") ||
                fieldName.OICEquals("Sexual"))
            {
                fixedProperty = FixedProperty.Gender;
                propertyType = "Gender";
                fieldName = "Gender";
                fieldHumanizeName = "性别";
                return;
            }
        }
        else if (propertyType == "long")
        {
            if (fieldName == "排序" ||
                fieldName.OICEquals("Order") ||
                fieldName.OICEquals("Sort"))
            {
                fixedProperty = FixedProperty.Order;
                fieldName = "Order";
                fieldHumanizeName = "排序";
                return;
            }
        }
        else if (propertyType == "bool")
        {
            if (fieldName == "是否置顶" ||
                fieldName == "置顶" ||
                fieldName.OICEquals("IsTop"))
            {
                fixedProperty = FixedProperty.IsTop;
                fieldName = "IsTop";
                fieldHumanizeName = "是否置顶";
                return;
            }
            else if (fieldName == "是否禁用" ||
                fieldName == "禁用" ||
                fieldName == "是否启用" ||
                fieldName == "启用" ||
                fieldName.OICEquals("Enable") ||
                fieldName.OICEquals("Enabled") ||
                fieldName.OICEquals("Disable") ||
                fieldName.OICEquals("Disabled") ||
                fieldName.OICEquals("IsEnable") ||
                fieldName.OICEquals("IsDisable"))
            {
                fixedProperty = FixedProperty.Disable;
                fieldName = "Disable";
                fieldHumanizeName = "是否禁用";
                return;
            }
            else if (fieldName == "软删除" ||
                fieldName == "是否软删除" ||
                fieldName == "是否删除" ||
                fieldName == "删除" ||
                fieldName.OICEquals("SoftDeleted") ||
                fieldName.OICEquals("IsDeleted") ||
                fieldName.OICEquals("Deleted") ||
                fieldName.OICEquals("IsDelete") ||
                fieldName.OICEquals("Delete"))
            {
                fixedProperty = FixedProperty.SoftDeleted;
                fieldName = "SoftDeleted";
                fieldHumanizeName = "是否软删除";
                return;
            }
        }
        else if (propertyType.IsGuidOrWithNullable())
        {
            if (fieldName == "租户Id" ||
                fieldName == "租户" ||
                fieldName.OICEquals("TenantId"))
            {
                fixedProperty = FixedProperty.TenantId;
                propertyType = "Guid";
                fieldName = "TenantId";
                fieldHumanizeName = "租户 Id";
                return;
            }
            else if (fieldName == "创建人" ||
                fieldName.OICEquals("CreateUserId"))
            {
                fixedProperty = FixedProperty.CreateUserId;
                propertyType = "Guid?";
                fieldName = "CreateUserId";
                fieldHumanizeName = "创建人（创建此条目的后台管理员）";
                return;
            }
            else if (fieldName == "操作人" ||
                fieldName == "编辑人" ||
                fieldName == "修改人" ||
                fieldName.OICEquals("OperatorUserId"))
            {
                fixedProperty = FixedProperty.OperatorUserId;
                propertyType = "Guid?";
                fieldName = "OperatorUserId";
                fieldHumanizeName = "最后一次操作的人（记录后台管理员禁用或启用或编辑该条的操作）";
                return;
            }
        }

        fixedProperty = default;
        fieldHumanizeName = fieldName.HasOther() ? fieldName.Humanize(LetterCasing.Title) : fieldName;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsDateTimeOrWithOffset(this string typeString)
        => typeString == "DateTimeOffset" ||
        typeString == "DateTime";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsStringOrWithNullable(this string typeString)
        => typeString == "string" ||
        typeString == "string?";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsGuidOrWithNullable(this string typeString)
        => typeString == "Guid" ||
        typeString == "Guid?";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsGenderOrWithNullable(this string typeString)
        => typeString == "Gender" ||
        typeString == "Gender?";
}
