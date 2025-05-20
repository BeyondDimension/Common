using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.ComponentModel;

/// <summary>
/// 类型转换绑定
/// </summary>
public static partial class TypeConverterBinder
{
    static readonly Dictionary<Type, Binder> binders = [];

    /// <summary>
    /// 绑定转换器到指定类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader"></param>
    /// <param name="writer"></param>
#if NET7_0_OR_GREATER
    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
#endif
    public static void Bind<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(Func<string, T?> reader, Func<T?, string?> writer)
    {
        binders[typeof(T)] = new Binder<T>(reader, writer);

        var converterType = typeof(TypeConverter<>).MakeGenericType(typeof(T));
        if (TypeDescriptor.GetConverter(typeof(T)).GetType() != converterType)
        {
            TypeDescriptor.AddAttributes(typeof(T), new TypeConverterAttribute(converterType));
        }
    }

    abstract class Binder
    {
        public abstract object? Read(string value);

        public abstract string? Write(object? value);
    }

    sealed class Binder<T>(Func<string, T?> reader, Func<T?, string?> writer) : Binder
    {
        readonly Func<string, T?> reader = reader;
        readonly Func<T?, string?> writer = writer;

        public override object? Read(string value)
        {
            return reader(value);
        }

        public override string? Write(object? value)
        {
            return writer((T?)value);
        }
    }

    sealed class TypeConverter<T> : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string stringVal)
            {
                if (stringVal.Equals(string.Empty))
                {
                    return default(T);
                }
                else if (binders.TryGetValue(typeof(T), out var binder))
                {
                    return binder.Read(stringVal);
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            return destinationType == typeof(T) && binders.TryGetValue(destinationType, out var binder)
                ? binder.Write(value)
                : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
