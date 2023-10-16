#pragma warning disable SA1600 // Elements should be documented

namespace System.ComponentModel;

public class EnumDescriptionTypeConverter([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields)] Type type) : EnumConverter(type)
{
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