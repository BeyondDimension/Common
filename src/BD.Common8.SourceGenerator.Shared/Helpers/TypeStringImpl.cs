using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace BD.Common8.SourceGenerator.Helpers;

/// <summary>
/// CLR 类型包装类，包装类型的完全限定名称，包括其命名空间，但不包括程序集的字符串 <see cref="Type.FullName"/> 或 Roslyn 编译器基于 C# 源码字符串分析的类型符号 <see cref="ITypeSymbol"/> 的实现 <see cref="Type"/> 包装类型，与 System.RuntimeType 不同，仅实现一小部分内容
/// </summary>
[DebuggerDisplay("{FullName,nq}")]
public sealed class TypeStringImpl : Type
{
    const string NotImplementedExceptionMessage = "类型 BD.Common8.SourceGenerator.Helpers.TypeStringImpl 仅提供字符串 与 Microsoft.CodeAnalysis.ITypeSymbol 到 System.Type 类的模拟，一些函数或属性尚未支持";

    readonly ITypeSymbol? typeSymbol;
    readonly INamedTypeSymbol? namedTypeSymbol;
    readonly string? lowerKeywordName;
    readonly string? globalFullName;
    readonly TypeCode typeCode;

    /// <inheritdoc cref="INamedTypeSymbol.TypeArguments"/>
    public ImmutableArray<TypeStringImpl> TypeArguments { get; }

    public TypeStringImpl(ITypeSymbol typeSymbol) : this(typeSymbol.ToDisplayString(), typeSymbol)
    {
    }

    readonly string toString;
    readonly string? dictionaryKey;
    readonly string? dictionaryValue;
    readonly string? genericT;

    public TypeStringImpl(string fullName, ITypeSymbol? typeSymbol = null)
    {
        this.typeSymbol = typeSymbol;
        FullName = fullName;
        (typeCode, lowerKeywordName, globalFullName) = GetTypeCode(fullName);

        // 是否已解析类型中的泛型参数类型
        bool resolvedGenericType = false;
        bool isGenericType = false;
        bool isUnboundGenericType = false;

        if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
        {
            this.namedTypeSymbol = namedTypeSymbol;
            if (isGenericType = namedTypeSymbol.IsGenericType)
            {
                if (!(isUnboundGenericType = namedTypeSymbol.IsUnboundGenericType))
                {
                    if (!namedTypeSymbol.TypeArguments.IsDefaultOrEmpty)
                    {
                        TypeArguments = [.. namedTypeSymbol.TypeArguments.Select(static x => new TypeStringImpl(x))];
                        genericT = dictionaryKey = TypeArguments.Length >= 1 ? TypeArguments[0].ToString() : null;
                        dictionaryValue = TypeArguments.Length >= 2 ? TypeArguments[1].ToString() : null;
                        resolvedGenericType = true;
                    }
                }
            }
        }

        toString = GetTypeWriteString(fullName, typeCode, lowerKeywordName, globalFullName, this.namedTypeSymbol, TypeArguments);

        if (!resolvedGenericType)
        {
            if (isGenericType)
            {
                if (isUnboundGenericType)
                {
                    dictionaryKey = dictionaryValue = genericT = string.Empty;
                }
                else
                {
                    dictionaryKey = GetDictionaryKey(toString);
                    dictionaryValue = GetDictionaryValue(toString);
                    genericT = GetGenericT(toString);
                }
            }
        }
    }

    public ITypeSymbol? TypeSymbol => typeSymbol;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeStringImpl? Parse(string? fullName) =>
        string.IsNullOrWhiteSpace(fullName) ? null : new TypeStringImpl(fullName!);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeStringImpl Parse(ITypeSymbol typeSymbol) =>
        new(typeSymbol);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ITypeSymbol? GetTypeSymbol(Type type)
    {
        if (type is TypeStringImpl typeStringImpl)
            return typeStringImpl.typeSymbol;
        return null;
    }

    static (TypeCode typeCode, string? lowerKeywordName, string? globalFullName) GetTypeCode(string fullName)
    {
        return fullName switch
        {
            "bool" or "global::System.Boolean" or "System.Boolean" => (TypeCode.Boolean, "bool", null),
            "char" or "global::System.Char" or "System.Char" => (TypeCode.Char, "char", null),
            "sbyte" or "global::System.SByte" or "System.SByte" => (TypeCode.SByte, "sbyte", null),
            "byte" or "global::System.Byte" or "System.Byte" => (TypeCode.Byte, "byte", null),
            "short" or "global::System.Int16" or "System.Int16" => (TypeCode.Int16, "short", null),
            "ushort" or "global::System.UInt16" or "System.UInt16" => (TypeCode.UInt16, "ushort", null),
            "int" or "global::System.Int32" or "System.Int32" => (TypeCode.Int32, "int", null),
            "nint" or "global::System.IntPtr" or "System.IntPtr" => (TypeCode.Object, "nint", null),
            "nuint" or "global::System.UIntPtr" or "System.UIntPtr" => (TypeCode.Object, "nuint", null),
            "uint" or "global::System.UInt32" or "System.UInt32" => (TypeCode.UInt32, "uint", null),
            "long" or "global::System.Int64" or "System.Int64" => (TypeCode.Int64, "long", null),
            "ulong" or "global::System.UInt64" or "System.UInt64" => (TypeCode.UInt64, "ulong", null),
            "float" or "global::System.Single" or "System.Single" => (TypeCode.Single, "float", null),
            "double" or "global::System.Double" or "System.Double" => (TypeCode.Double, "double", null),
            "decimal" or "global::System.Decimal" or "System.Decimal" => (TypeCode.Decimal, "decimal", null),
            "string" or "global::System.String" or "System.String" => (TypeCode.String, "string", null),
            "DateOnly" or "global::System.DateOnly" or "System.DateOnly" => (TypeCode.Object, null, "global::System.DateOnly"),
            "DateTime" or "global::System.DateTime" or "System.DateTime" => (TypeCode.DateTime, null, "global::System.DateTime"),
            "DateTimeOffset" or "global::System.DateTimeOffset" or "System.DateTimeOffset" => (TypeCode.Object, null, "global::System.DateTimeOffset"),
            "CancellationToken" or "global::System.Threading.CancellationToken" or "System.Threading.CancellationToken" => (TypeCode.Object, null, "global::System.Threading.CancellationToken"),
            _ => (TypeCode.Object, null, null),
        };
    }

    public TypeCode GetTypeCode() => typeCode;

    public bool IsSystemBoolean => typeCode == TypeCode.Boolean;

    public bool IsSystemString => typeCode == TypeCode.String;

    public bool IsSystemDateOnly => globalFullName == "global::System.DateOnly";

    public bool IsSystemDateTime => typeCode == TypeCode.DateTime;

    public bool IsSystemDateTimeOffset => globalFullName == "global::System.DateTimeOffset";

    public bool IsSystemThreadingCancellationToken => globalFullName == "global::System.Threading.CancellationToken";

#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    const char RightBracket = '>';
#else
    const string RightBracket = ">";
#endif

    public bool IsSystemCollectionsGenericIAsyncEnumerable =>
        FullName.StartsWith("System.Collections.Generic.IAsyncEnumerable<") && FullName.EndsWith(RightBracket);

    static string GetTypeWriteString(string fullName)
    {
        (var typeCode, var lowerKeywordName, var globalFullName) = GetTypeCode(fullName);
        return GetTypeWriteString(fullName, typeCode, lowerKeywordName, globalFullName, default, default);
    }

    static string GetTypeWriteString(string fullName, TypeCode typeCode, string? lowerKeywordName, string? globalFullName, INamedTypeSymbol? namedTypeSymbol, ImmutableArray<TypeStringImpl> typeArguments = default)
    {
        if (lowerKeywordName != null)
        {
            return lowerKeywordName;
        }
        else if (globalFullName != null)
        {
            return globalFullName;
        }
        if (namedTypeSymbol != null && !typeArguments.IsDefaultOrEmpty && namedTypeSymbol.IsGenericType && !namedTypeSymbol.IsUnboundGenericType)
        {
            var indexL = fullName.IndexOf('<');
            if (indexL != -1 && fullName.Contains('>'))
            {
                StringBuilder builder = new();
                if (!fullName.StartsWith("global::"))
                {
                    builder.Append("global::");
                }
                builder.Append(fullName, 0, indexL);
                builder.Append('<');
                for (int i = 0; i < typeArguments.Length; i++)
                {
                    var it = typeArguments[i];
                    builder.Append(it.ToString());
                    if (i != typeArguments.Length - 1)
                    {
                        builder.Append(", ");
                    }
                }
                builder.Append('>');
                return builder.ToString();
            }
        }
        if (fullName.Contains('.'))
        {
            if (!fullName.StartsWith("global::"))
            {
                return $"global::{fullName}";
            }
        }
        return fullName;
    }

    static string? GetDictionaryKey(string name)
    {
        try
        {
            var indexL = name.IndexOf('<');
            if (indexL == -1)
            {
                return null;
            }
            var indexR = name.IndexOf(',');
            if (indexR == -1)
            {
                return null;
            }
            var result = name.Substring(indexL + 1, indexR - indexL - 1);
            var result2 = GetTypeWriteString(result);
            return result2;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }

    static string? GetDictionaryValue(string name)
    {
        try
        {
            var indexL = name.IndexOf(',');
            if (indexL == -1)
            {
                return null;
            }
            indexL++;
            for (int i = 0; i < byte.MaxValue; i++)
            {
                if (name[indexL] == ' ')
                {
                    indexL += 1;
                }
                else
                {
                    break;
                }
            }
            var indexR = name.Length - 1;
            if (name[indexR] == '?')
            {
                indexR -= 1;
            }
            if (name[indexR] == '>')
            {
                indexR -= 1;
            }
            var result = name[indexL..(indexR + 1)];
            var result2 = GetTypeWriteString(result);
            return result2;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }

    static string? GetGenericT(string name)
    {
        try
        {
            var indexR = name.IndexOf('<');
            if (indexR == -1)
            {
                return null;
            }
            indexR++;
            var len = name.Length;
            if (name[len - 1] == '?')
            {
                len -= 1;
            }
            if (name[len - 1] == '>')
            {
                len -= 1;
            }
            var result = name[indexR..len];
            var result2 = GetTypeWriteString(result);
            return result2;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }

    public string DictionaryKey
    {
        get
        {
            if (dictionaryKey == null)
            {
                throw new ArgumentNullException($"参数 dictionaryKey 值不能为 null, toString= {toString}");
            }
            return dictionaryKey;
        }
    }

    public string DictionaryValue
    {
        get
        {
            if (dictionaryValue == null)
            {
                throw new ArgumentNullException($"参数 dictionaryValue 值不能为 null, toString= {toString}");
            }
            return dictionaryValue;
        }
    }

    public string GenericT
    {
        get
        {
            if (genericT == null)
            {
                throw new ArgumentNullException($"参数 genericT 值不能为 null, toString= {toString}");
            }
            return genericT;
        }
    }

    public override Assembly Assembly => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override string AssemblyQualifiedName => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override Type BaseType => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override string FullName { get; }

    public override Guid GUID => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override Module Module => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override string Namespace
    {
        get
        {
            var array = FullName.Split(['.'], StringSplitOptions.RemoveEmptyEntries);
            var result = string.Join(".", array.Take(array.Length - 1));
            return result;
        }
    }

    public override Type UnderlyingSystemType => throw new NotImplementedException(NotImplementedExceptionMessage);

    string? name;

    static string GetNameByFullName(string fullName)
    {
        foreach (var item in KeepFullNames)
        {
            if (fullName.Contains(item))
            {
                return fullName;
            }
        }
        try
        {
            var split = fullName.Split(['<', '>', ',']);
            StringBuilder builder = new();
            var index = 0;
            for (int i = 0; i < split.Length; i++)
            {
                var item = split[i];
                var itemSplitLast = item.Trim().Split(['.'],
                    StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                if (item.FirstOrDefault() == ' ')
                    builder.Append(' ');
                builder.Append(itemSplitLast);
                index += item.Length;
                if (i != 0)
                    index += 1;
                if (index < fullName.Length)
                    builder.Append(fullName[index]);
            }
            var name = builder.ToString();
            return name;
        }
        catch
        {
            return fullName;
        }
    }

    public override string Name => name ??= GetNameByFullName(FullName);

    public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override object[] GetCustomAttributes(bool inherit) => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override Type GetElementType() => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override EventInfo GetEvent(string name, BindingFlags bindingAttr) => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override EventInfo[] GetEvents(BindingFlags bindingAttr) => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override FieldInfo GetField(string name, BindingFlags bindingAttr) => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override FieldInfo[] GetFields(BindingFlags bindingAttr) => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override Type GetInterface(string name, bool ignoreCase) => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override Type[] GetInterfaces() => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override MemberInfo[] GetMembers(BindingFlags bindingAttr) => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override MethodInfo[] GetMethods(BindingFlags bindingAttr) => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override Type GetNestedType(string name, BindingFlags bindingAttr) => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override Type[] GetNestedTypes(BindingFlags bindingAttr) => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override PropertyInfo[] GetProperties(BindingFlags bindingAttr) => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override bool IsDefined(Type attributeType, bool inherit) => throw new NotImplementedException(NotImplementedExceptionMessage);

    protected override TypeAttributes GetAttributeFlagsImpl() => throw new NotImplementedException(NotImplementedExceptionMessage);

    protected override bool HasElementTypeImpl() => throw new NotImplementedException(NotImplementedExceptionMessage);

    protected override bool IsArrayImpl() => throw new NotImplementedException(NotImplementedExceptionMessage);

    protected override bool IsByRefImpl() => throw new NotImplementedException(NotImplementedExceptionMessage);

    protected override bool IsCOMObjectImpl() => throw new NotImplementedException(NotImplementedExceptionMessage);

    protected override bool IsPointerImpl() => throw new NotImplementedException(NotImplementedExceptionMessage);

    protected override bool IsPrimitiveImpl() => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override string ToString() => toString;

    protected override ConstructorInfo? GetConstructorImpl(BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[] types, ParameterModifier[]? modifiers) => throw new NotImplementedException(NotImplementedExceptionMessage);

    protected override MethodInfo? GetMethodImpl(string name, BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[]? types, ParameterModifier[]? modifiers) => throw new NotImplementedException(NotImplementedExceptionMessage);

    protected override PropertyInfo? GetPropertyImpl(string name, BindingFlags bindingAttr, Binder? binder, Type? returnType, Type[]? types, ParameterModifier[]? modifiers) => throw new NotImplementedException(NotImplementedExceptionMessage);

    public override object? InvokeMember(string name, BindingFlags invokeAttr, Binder? binder, object? target, object?[]? args, ParameterModifier[]? modifiers, CultureInfo? culture, string[]? namedParameters) => throw new NotImplementedException(NotImplementedExceptionMessage);

    static readonly string[] KeepFullNames = [
        "Color",
        "Size",
        "Point",
        "Vector",
        "Bitmap",
        "Visual",
        "Orientation",
        "Brushes",
        "FontFamily",
        "PixelFormat",
        "MouseButton",
        "ProgressBar",
        "Button",
        "Notification",
        "Controls",
        "Rectangle",
        "HorizontalAlignment",
        "VerticalAlignment",
    ];
}