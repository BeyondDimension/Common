namespace BD.Common8.SourceGenerator.Helpers;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 包装 <see cref="string"/> 或 <see cref="ITypeSymbol"/> 到 <see cref="Type"/> 的类型转换
/// </summary>
/// <param name="fullName"></param>
public sealed class TypeStringImpl(string fullName) : Type
{
    readonly ITypeSymbol? typeSymbol;

    public TypeStringImpl(ITypeSymbol typeSymbol) : this(typeSymbol.ToDisplayString())
    {
        this.typeSymbol = typeSymbol;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Type? Parse(string? fullName) =>
        string.IsNullOrWhiteSpace(fullName) ? null : new TypeStringImpl(fullName!);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Type Parse(ITypeSymbol typeSymbol) =>
        new TypeStringImpl(typeSymbol);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ITypeSymbol? GetTypeSymbol(Type type)
    {
        if (type is TypeStringImpl typeStringImpl)
            return typeStringImpl.typeSymbol;
        return null;
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

    public override string Name => FullName.Split(['.'], StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

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

    public override string ToString() => FullName;
}