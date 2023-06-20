namespace BD.Common.Repositories.SourceGenerator.Handlers.Attributes;

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
            else if (double.Epsilon == d)
            {
                return "double.Epsilon"u8.ToArray();
            }
            else if (double.MaxValue == d)
            {
                return "double.MaxValue"u8.ToArray();
            }
            else if (double.MinValue == d)
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
            else if (float.Epsilon == f)
            {
                return "float.Epsilon"u8.ToArray();
            }
            else if (float.MaxValue == f)
            {
                return "float.MaxValue"u8.ToArray();
            }
            else if (float.MinValue == f)
            {
                return "float.MinValue"u8.ToArray();
            }
            return $"{f}f";
        }
        else if (value is decimal dec)
        {
            if (decimal.MaxValue == dec)
            {
                return "decimal.MaxValue"u8.ToArray();
            }
            else if (decimal.MinValue == dec)
            {
                return "decimal.MinValue"u8.ToArray();
            }
            return $"{dec}m";
        }
        else if (value is sbyte @sbyte)
        {
            if (sbyte.MaxValue == @sbyte)
            {
                return "sbyte.MaxValue"u8.ToArray();
            }
            else if (sbyte.MinValue == @sbyte)
            {
                return "sbyte.MinValue"u8.ToArray();
            }
            return @sbyte.ToString();
        }
        else if (value is byte @byte)
        {
            if (byte.MaxValue == @byte)
            {
                return "byte.MaxValue"u8.ToArray();
            }
            else if (byte.MinValue == @byte)
            {
                return "byte.MinValue"u8.ToArray();
            }
            return @byte.ToString();
        }
        else if (value is short @short)
        {
            if (short.MaxValue == @short)
            {
                return "short.MaxValue"u8.ToArray();
            }
            else if (short.MinValue == @short)
            {
                return "short.MinValue"u8.ToArray();
            }
            return @short.ToString();
        }
        else if (value is ushort @ushort)
        {
            if (ushort.MaxValue == @ushort)
            {
                return "ushort.MaxValue"u8.ToArray();
            }
            else if (ushort.MinValue == @ushort)
            {
                return "ushort.MinValue"u8.ToArray();
            }
            return @ushort.ToString();
        }
        else if (value is int @int)
        {
            if (int.MaxValue == @int)
            {
                return "int.MaxValue"u8.ToArray();
            }
            else if (int.MinValue == @int)
            {
                return "int.MinValue"u8.ToArray();
            }
            return @int.ToString();
        }
        else if (value is uint @uint)
        {
            if (uint.MaxValue == @uint)
            {
                return "uint.MaxValue"u8.ToArray();
            }
            else if (uint.MinValue == @uint)
            {
                return "uint.MinValue"u8.ToArray();
            }
            return @uint.ToString();
        }
        else if (value is long @long)
        {
            if (long.MaxValue == @long)
            {
                return "long.MaxValue"u8.ToArray();
            }
            else if (long.MinValue == @long)
            {
                return "long.MinValue"u8.ToArray();
            }
            return @long.ToString();
        }
        else if (value is ulong @ulong)
        {
            if (ulong.MaxValue == @ulong)
            {
                return "ulong.MaxValue"u8.ToArray();
            }
            else if (ulong.MinValue == @ulong)
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

    void Write(Stream s, AttributeData attribute)
    {
        var anyConstructorArguments = attribute.ConstructorArguments.Any();
        var anyNamedArguments = attribute.NamedArguments.Any();
        if (anyConstructorArguments || anyNamedArguments)
        {
            s.Write("("u8);
            if (anyConstructorArguments)
            {
                for (int i = 0; i < attribute.ConstructorArguments.Length; i++)
                {
                    var item = attribute.ConstructorArguments[i];
                    s.WriteObject(GetValue(item.GetObjectValue()));
                    if (i != attribute.ConstructorArguments.Length - 1)
                        s.Write(", "u8);
                }
            }
            if (anyNamedArguments)
            {
                s.Write(", "u8);
                for (int i = 0; i < attribute.NamedArguments.Length; i++)
                {
                    var item = attribute.NamedArguments[i];
                    s.Write(item.Key);
                    s.Write(" = "u8);
                    s.WriteObject(GetValue(item.Value.GetObjectValue()));
                    if (i != attribute.ConstructorArguments.Length - 1)
                        s.Write(", "u8);
                }
            }
            s.Write(")"u8);
        }
    }

    string? IAttributeHandle.Write(AttributeHandleArguments args)
    {
        if (GeneratorConfig.AttrTypeFullNames.TryGetValue(args.AttributeClassFullName, out var attrShortName))
        {
            if (args.ClassType != ClassType.Entities)
            {
                // 非实体类型不需要 EFCore 的特性
                if (args.AttributeClassFullName.StartsWith("Microsoft.EntityFrameworkCore."))
                    return args.AttributeClassFullName;
            }

            var template0 =
"""
    [
"""u8;
            args.Stream.Write(template0);
            args.Stream.Write(attrShortName);

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
            Write(args.Stream, args.Attribute);

            var template1 =
"""
]

"""u8;
            args.Stream.Write(template1);
            return args.AttributeClassFullName;
        }
        return default;
    }
}
