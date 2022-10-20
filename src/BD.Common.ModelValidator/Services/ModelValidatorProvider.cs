/* (模型验证提供者)模块说明
 * 模型类 应当继承接口 IReadOnlyXXX 或 IReadOnlyNullableXXX
 * IXXX接口 不继承 IReadOnly 或 IReadOnlyNullable
 * 所以 应当 使用 IReadOnlyXXX 或 IReadOnlyNullableXXX
 * IXXX 实现 get, set 常用于表中的字段
 * IReadOnlyXXX 仅实现get，用于验证字段值是否正确
 * IReadOnlyNullableXXX 基本同上，不同在于允许 null 值，如果字段类型有Empty，则通常Empty等效于null
 * ----------------------------------------------------------------------
 * 添加一个新的列的验证，例：
 * 定义一个 IXXX接口
 * 定义一个 IReadOnlyXXX接口
 * 如果允许null值，则再定义一个 IReadOnlyNullableXXX 接口
 * ----------------------------------------------------------------------
 * 验证过滤字段参数 params Type[] ignores
 * 传入的类型 为接口 IXXX 或 IReadOnlyXXX 或 IReadOnlyNullableXXX 三者等效
 * 实现逻辑：通过反射Name字符串截取获取XXX作为比较
 * ----------------------------------------------------------------------
 * 对于模型类中某一个字段XXX，要么此类继承 IReadOnlyXXX 要么继承 IReadOnlyNullableXXX 不能同时继承两者
 * ----------------------------------------------------------------------
 * 如果对于某一个模型类中的某个类型，需要对另一个字段进行不同的验证，可添加新的类型字段接口
 * 在 ColumnValidate 验证逻辑中 添加新的 is IXXX t switch(t.X) 进行不同的验证
 * ----------------------------------------------------------------------
 * 如果需要根据不同的模型类，进行输出不同的 errorMessage 原理同上
 */
using static BD.Common.Services.Implementation.ModelValidator;

namespace BD.Common.Services;

public static class ModelValidatorProvider
{
    public static void Init()
    {
        AddColumnValidate<IExplicitHasValue>(x =>
        {
            if (!x.HasValue()) return SR.内容值不能为空或不正确;
            return null;
        });
        AddColumnValidate<IReadOnlyPhoneNumber>(ColumnValidate);
        AddColumnValidate<IReadOnlySmsCode>(ColumnValidate);
        //AddColumnValidate<IReadOnlyBirthDate>(ColumnValidate);
        //AddColumnValidate<IReadOnlyGender>(ColumnValidate);
        AddColumnValidate<IReadOnlyAvatar>(ColumnValidate);
        AddColumnValidate<IReadOnlyNickName>(ColumnValidate);
    }

    #region ColumnValidates

    static string? ColumnValidate(IReadOnlyPhoneNumber value)
    {
        if (string.IsNullOrEmpty(value.PhoneNumber))
        {
            return SR.请输入手机号码哦;
        }
        if (!IsPhoneNumberCorrect(value.PhoneNumber))
        {
            return SR.请输入正确的手机号码哦;
        }
        return null;
    }

    static string? ColumnValidate(IReadOnlySmsCode value)
    {
        if (string.IsNullOrEmpty(value.SmsCode))
        {
            return SR.请输入短信验证码哦;
        }
        if (!IsSmsCodeCorrect(value.SmsCode))
        {
            return SR.短信验证码不正确;
        }
        return null;
    }

    //static string? ColumnValidate(IReadOnlyBirthDate value)
    //{
    //    if (value.BirthDate == default)
    //    {
    //        return Constants.请输入年龄;
    //    }
    //    return null;
    //}

    //static string? ColumnValidate(IReadOnlyGender value)
    //{
    //    if (!value.Gender.IsDefined())
    //    {
    //        return Constants.请选择性别;
    //    }
    //    return null;
    //}

    static string? ColumnValidate(IReadOnlyAvatar value)
    {
        if (value.Avatar == default)
        {
            return SR.请选择头像;
        }
        return null;
    }

    static string? ColumnValidate(IReadOnlyNickName value)
    {
        if (string.IsNullOrWhiteSpace(value.NickName))
        {
            return SR.请输入昵称;
        }
        else if (value.NickName.Length > MaxLengths.NickName)
        {
            return SR.昵称最大长度不能超过_.Format(MaxLengths.NickName);
        }
        return null;
    }

    #endregion

    #region Validates

    /// <summary>
    /// 验证字符串是否为正确的手机号码
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    static bool IsPhoneNumberCorrect(string value)
        // 纯数字， 1开头，11位手机号码
        => !(value.Length != PhoneNumberHelper.ChineseMainlandPhoneNumberLength || value[0] != '1' || !value.IsDigital());

    /// <summary>
    /// 验证字符串是否为正确的短信验证码
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    static bool IsSmsCodeCorrect(string value)
        => !(value.Length != MaxLengths.SMS_CAPTCHA || !value.IsDigital());

    public static bool IsPortId(ushort value) => value != 0;

    public static bool IsPortId(int value) => value > 0 && value <= ushort.MaxValue;

    public static bool IsPortId(string value, out ushort value2) => ushort.TryParse(value, out value2) && IsPortId(value2);

    #endregion
}
