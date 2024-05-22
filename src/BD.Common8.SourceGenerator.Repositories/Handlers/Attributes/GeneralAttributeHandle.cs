namespace BD.Common8.SourceGenerator.Repositories.Handlers.Attributes;

sealed class GeneralAttributeHandle : IAttributeHandle
{
    public static IAttributeHandle Instance { get; } = new GeneralAttributeHandle();

    static object? GetValue(object? value)
    {
        if (value is double d)
        {
            if (double.IsNaN(d))
            {
                return "double.NaN"u8.ToArray();
            }
            else if (double.IsNegativeInfinity(d))
            {
                return "double.NegativeInfinity"u8.ToArray();
            }
            else if (double.IsPositiveInfinity(d))
            {
                return "double.PositiveInfinity"u8.ToArray();
            }
            else if (d == double.Epsilon)
            {
                return "double.Epsilon"u8.ToArray();
            }
            else if (d == double.MaxValue)
            {
                return "double.MaxValue"u8.ToArray();
            }
            else if (d == double.MinValue)
            {
                return "double.MinValue"u8.ToArray();
            }
            return $"{d}d";
        }
        else if (value is float f)
        {
            if (float.IsNaN(f))
            {
                return "float.NaN"u8.ToArray();
            }
            else if (float.IsNegativeInfinity(f))
            {
                return "float.NegativeInfinity"u8.ToArray();
            }
            else if (float.IsPositiveInfinity(f))
            {
                return "float.PositiveInfinity"u8.ToArray();
            }
            else if (f == float.Epsilon)
            {
                return "float.Epsilon"u8.ToArray();
            }
            else if (f == float.MaxValue)
            {
                return "float.MaxValue"u8.ToArray();
            }
            else if (f == float.MinValue)
            {
                return "float.MinValue"u8.ToArray();
            }
            return $"{f}f";
        }
        else if (value is decimal dec)
        {
            if (dec == decimal.MaxValue)
            {
                return "decimal.MaxValue"u8.ToArray();
            }
            else if (dec == decimal.MinValue)
            {
                return "decimal.MinValue"u8.ToArray();
            }
            return $"{dec}m";
        }
        else if (value is sbyte @sbyte)
        {
            if (@sbyte == sbyte.MaxValue)
            {
                return "sbyte.MaxValue"u8.ToArray();
            }
            else if (@sbyte == sbyte.MinValue)
            {
                return "sbyte.MinValue"u8.ToArray();
            }
            return @sbyte.ToString();
        }
        else if (value is byte @byte)
        {
            if (@byte == byte.MaxValue)
            {
                return "byte.MaxValue"u8.ToArray();
            }
            else if (@byte == byte.MinValue)
            {
                return "byte.MinValue"u8.ToArray();
            }
            return @byte.ToString();
        }
        else if (value is short @short)
        {
            if (@short == short.MaxValue)
            {
                return "short.MaxValue"u8.ToArray();
            }
            else if (@short == short.MinValue)
            {
                return "short.MinValue"u8.ToArray();
            }
            return @short.ToString();
        }
        else if (value is ushort @ushort)
        {
            if (@ushort == ushort.MaxValue)
            {
                return "ushort.MaxValue"u8.ToArray();
            }
            else if (@ushort == ushort.MinValue)
            {
                return "ushort.MinValue"u8.ToArray();
            }
            return @ushort.ToString();
        }
        else if (value is int @int)
        {
            if (@int == int.MaxValue)
            {
                return "int.MaxValue"u8.ToArray();
            }
            else if (@int == int.MinValue)
            {
                return "int.MinValue"u8.ToArray();
            }
            return @int.ToString();
        }
        else if (value is uint @uint)
        {
            if (@uint == uint.MaxValue)
            {
                return "uint.MaxValue"u8.ToArray();
            }
            else if (@uint == uint.MinValue)
            {
                return "uint.MinValue"u8.ToArray();
            }
            return @uint.ToString();
        }
        else if (value is long @long)
        {
            if (@long == long.MaxValue)
            {
                return "long.MaxValue"u8.ToArray();
            }
            else if (@long == long.MinValue)
            {
                return "long.MinValue"u8.ToArray();
            }
            return @long.ToString();
        }
        else if (value is ulong @ulong)
        {
            if (@ulong == ulong.MaxValue)
            {
                return "ulong.MaxValue"u8.ToArray();
            }
            else if (@ulong == ulong.MinValue)
            {
                return "ulong.MinValue"u8.ToArray();
            }
            return @ulong.ToString();
        }
        else if (value is string s)
        {
            return $"\"{s.Replace('"'.ToString(), "\\" + '"')}\"";
        }
        else
        {
            if (value is INamedTypeSymbol symbol)
            {
                if (symbol.IsType)
                {
                    return $"typeof({symbol.ToDisplayString()})";
                }
            }
            return value?.ToString()?.TrimStart("System.ComponentModel.DataAnnotations.Schema.");
        }
    }

    void Write(Stream s, string? attributeValue)
    {
        if (attributeValue == null)
            return;

        var formatStr =
"""
({0})
"""u8;
        s.WriteFormat(formatStr, attributeValue);
    }

    string? IAttributeHandle.Write(AttributeHandleArguments args)
    {
        if (GeneratorConfig.AttrTypeFullNames.TryGetValue(args.AttributeName, out var attributeClassFullName))
        {
            if (args.ClassType != ClassType.Entities)
            {
                // 非实体类型不需要 EFCore 的特性
                if (attributeClassFullName.StartsWith("Microsoft.EntityFrameworkCore."))
                    return attributeClassFullName;
            }

            var template0 =
"""
            [
        """u8;
            args.Stream.Write(template0);
            args.Stream.WriteUtf16StrToUtf8OrCustom(args.AttributeName);

            //var value = args.Attribute.ToString()
            //    .Replace("System.ComponentModel.DataAnnotations.Schema.", string.Empty);
            //var l_brace_index = value.IndexOf('(');
            //if (l_brace_index >= 0)
            //{
            //    var bytes = Encoding.UTF8.GetBytes(
            //        value.ToCharArray(),
            //        l_brace_index,
            //        value.Length - l_brace_index);
            //    args.Stream.Write(bytes);
            //}
            if (args.AttributeValue != "True")
            {
                Write(args.Stream, args.AttributeValue);
            }

            var template1 =
"""
        ]

        """u8;
            args.Stream.Write(template1);
            return attributeClassFullName;
        }
        return default;
    }
}