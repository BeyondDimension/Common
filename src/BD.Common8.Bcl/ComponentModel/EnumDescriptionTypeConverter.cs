namespace System.ComponentModel;

/// <summary>
/// 用于将枚举类型转换为描述字符串
/// </summary>
/// <param name="type">需要进行转换的枚举类型，可以指定动态访问的成员类型（公共构造函数和公共字段）</param>
public class EnumDescriptionTypeConverter([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields)] Type type) : EnumConverter(type)
{
    /// <summary>
    /// 获取枚举值字段上的 DescriptionAttribute 特性来返回描述字符串，不存在则返回枚举值的字符串
    /// </summary>
    /// <returns></returns>
    public override object? ConvertTo(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        object? value,
        Type destinationType)
    {
        if (destinationType == typeof(string))
        {
            if (value != null)
            {
                var valueStr = value.ToString();
                if (!string.IsNullOrEmpty(valueStr))
                {
                    var fi = value.GetType().GetField(valueStr);
                    if (fi != null)
                    {
                        var attributes =
                            (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                        return ((attributes.Length > 0) && (!string.IsNullOrEmpty(attributes[0].Description)))
                            ? attributes[0].Description
                            : value.ToString();
                    }
                }
            }

            return string.Empty;
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }
}