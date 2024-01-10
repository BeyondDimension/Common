namespace BD.Common8.SourceGenerator.Helpers;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 包装 <see cref="string"/> 或 <see cref="ITypeSymbol"/> 到 <see cref="Type"/> 的类型转换
/// </summary>
/// <param name="fullName"></param>
[DebuggerDisplay("{FullName,nq}")]
public sealed class TypeStringImpl(string fullName) : Type
{
    ITypeSymbol? typeSymbol;

    public TypeStringImpl(ITypeSymbol typeSymbol) : this(typeSymbol.ToDisplayString())
    {
        this.typeSymbol = typeSymbol;
    }

    public ITypeSymbol? TypeSymbol { get => typeSymbol; set => typeSymbol = value; }

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

    public bool IsSystemBoolean => FullName == "System.Boolean" || FullName == "bool";

    public bool IsSystemString => FullName == "System.String" || FullName == "string";

    public bool IsSystemDateOnly => FullName == "System.DateOnly" || FullName == "DateOnly";

    public bool IsSystemDateTime => FullName == "System.DateTime" || FullName == "DateTime";

    public bool IsSystemDateTimeOffset => FullName == "System.DateTimeOffset" || FullName == "DateTimeOffset";

    public bool IsSystemThreadingCancellationToken => FullName == "System.Threading.CancellationToken" || FullName == "CancellationToken";

    public bool IsSystemCollectionsGenericIAsyncEnumerable =>
        FullName.StartsWith("System.Collections.Generic.IAsyncEnumerable<") && FullName.EndsWith(">");

    public string DictionaryKey
    {
        get
        {
            try
            {
                var name = Name;
                var indexL = name.IndexOf('<');
                var indexR = name.IndexOf(',');
                var result = name.Substring(indexL + 1, indexR - indexL - 1);
                return result;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }

    public string DictionaryValue
    {
        get
        {
            try
            {
                var name = Name;
                var indexL = name.IndexOf(',') + 1;
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
                return result;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }

    public string GenericT
    {
        get
        {
            try
            {
                var name = Name;
                var indexR = name.IndexOf('<') + 1;
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
                return result;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }

    public override Assembly Assembly => throw new NotImplementedException();

    public override string AssemblyQualifiedName => throw new NotImplementedException();

    public override Type BaseType => throw new NotImplementedException();

    public override string FullName { get; } = fullName;

    public override Guid GUID => throw new NotImplementedException();

    public override Module Module => throw new NotImplementedException();

    public override string Namespace
    {
        get
        {
            var array = FullName.Split(['.'], StringSplitOptions.RemoveEmptyEntries);
            var result = string.Join(".", array.Take(array.Length - 1));
            return result;
        }
    }

    public override Type UnderlyingSystemType => throw new NotImplementedException();

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

    public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) => throw new NotImplementedException();

    public override object[] GetCustomAttributes(bool inherit) => throw new NotImplementedException();

    public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw new NotImplementedException();

    public override Type GetElementType() => throw new NotImplementedException();

    public override EventInfo GetEvent(string name, BindingFlags bindingAttr) => throw new NotImplementedException();

    public override EventInfo[] GetEvents(BindingFlags bindingAttr) => throw new NotImplementedException();

    public override FieldInfo GetField(string name, BindingFlags bindingAttr) => throw new NotImplementedException();

    public override FieldInfo[] GetFields(BindingFlags bindingAttr) => throw new NotImplementedException();

    public override Type GetInterface(string name, bool ignoreCase) => throw new NotImplementedException();

    public override Type[] GetInterfaces() => throw new NotImplementedException();

    public override MemberInfo[] GetMembers(BindingFlags bindingAttr) => throw new NotImplementedException();

    public override MethodInfo[] GetMethods(BindingFlags bindingAttr) => throw new NotImplementedException();

    public override Type GetNestedType(string name, BindingFlags bindingAttr) => throw new NotImplementedException();

    public override Type[] GetNestedTypes(BindingFlags bindingAttr) => throw new NotImplementedException();

    public override PropertyInfo[] GetProperties(BindingFlags bindingAttr) => throw new NotImplementedException();

    public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters) => throw new NotImplementedException();

    public override bool IsDefined(Type attributeType, bool inherit) => throw new NotImplementedException();

    protected override TypeAttributes GetAttributeFlagsImpl() => throw new NotImplementedException();

    protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) => throw new NotImplementedException();

    protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) => throw new NotImplementedException();

    protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers) => throw new NotImplementedException();

    protected override bool HasElementTypeImpl() => throw new NotImplementedException();

    protected override bool IsArrayImpl() => throw new NotImplementedException();

    protected override bool IsByRefImpl() => throw new NotImplementedException();

    protected override bool IsCOMObjectImpl() => throw new NotImplementedException();

    protected override bool IsPointerImpl() => throw new NotImplementedException();

    protected override bool IsPrimitiveImpl() => throw new NotImplementedException();

    public override string ToString() => Name;

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