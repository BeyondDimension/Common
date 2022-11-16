// ReSharper disable once CheckNamespace
namespace System.ComponentModel;

/// <summary>
/// TypeConverter(类型转换) 绑定
/// <para>https://github.com/dotnetcore/FastGithub/blob/2.1.4/FastGithub.Configuration/TypeConverterBinder.cs</para>
/// </summary>
public static class TypeConverterBinder
{
    static readonly Dictionary<Type, Binder> binders = new();

    /// <summary>
    /// 绑定转换器到指定类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader"></param>
    /// <param name="writer"></param>
    public static void Bind<T>(Func<string, T?> reader, Func<T?, string?> writer)
    {
        binders[typeof(T)] = new Binder<T>(reader, writer);

        var converterType = typeof(TypeConverter<>).MakeGenericType(typeof(T));
#pragma warning disable IL2087 // Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The generic parameter of the source method or type does not have matching annotations.
        if (TypeDescriptor.GetConverter(typeof(T)).GetType() != converterType)
        {
            TypeDescriptor.AddAttributes(typeof(T), new TypeConverterAttribute(converterType));
        }
#pragma warning restore IL2087 // Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The generic parameter of the source method or type does not have matching annotations.
    }

    abstract class Binder
    {
        public abstract object? Read(string value);

        public abstract string? Write(object? value);
    }

    sealed class Binder<T> : Binder
    {
        private readonly Func<string, T?> reader;
        private readonly Func<T?, string?> writer;

        public Binder(Func<string, T?> reader, Func<T?, string?> writer)
        {
            this.reader = reader;
            this.writer = writer;
        }

        public override object? Read(string value)
        {
            return this.reader(value);
        }

        public override string? Write(object? value)
        {
            return this.writer((T?)value);
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
