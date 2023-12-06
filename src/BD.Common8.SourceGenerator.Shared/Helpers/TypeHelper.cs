namespace BD.Common8.SourceGenerator.Helpers;

/// <summary>
/// 提供类型判断
/// </summary>
public static partial class TypeHelper
{
    /// <summary>
    /// 判断指定的类型是否为枚举类型
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEnum(this ITypeSymbol typeSymbol) => typeSymbol.TypeKind == TypeKind.Enum;

    /// <summary>
    /// 判断指定的类型是否为简单类型
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSimpleTypes(this ITypeSymbol typeSymbol)
    {
        // https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-8.0#simple-types

        if (typeSymbol.IsEnum())
            return true;

        switch (typeSymbol.ContainingNamespace.Name)
        {
            case "System":
                switch (typeSymbol.Name)
                {
                    case "Boolean":
                    case "Byte":
                    case "SByte":
                    case "Char":
                    case "DateOnly":
                    case "DateTime":
                    case "DateTimeOffset":
                    case "Decimal":
                    case "Double":
                    case "Guid":
                    case "Int16":
                    case "Int32":
                    case "Int64":
                    case "Single":
                    case "TimeOnly":
                    case "TimeSpan":
                    case "UInt16":
                    case "UInt32":
                    case "UInt64":
                    case "Uri":
                    case "Version":
                    case "String":
                        return true;
                }
                break;
        }

        return false;
    }

    /// <summary>
    /// 获取类型短名称
    /// </summary>
    /// <param name="typeSymbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetTypeShortName(this ITypeSymbol typeSymbol)
    {
        var typeString = typeSymbol.ToDisplayString();
        if (typeString.Contains('.'))
        {
            typeString = typeString.Split(['.'], StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        }
        return typeString;
    }

    /// <summary>
    /// 判断该函数是否为重写的 <see cref="object.ToString"/>
    /// </summary>
    /// <param name="methodSymbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsObjectToString(this IMethodSymbol methodSymbol) => methodSymbol.Name == nameof(ToString) &&
                    methodSymbol.ReturnType.Name == "String" &&
                    methodSymbol.Parameters.Length == 0;

    /// <summary>
    /// 判断该属性是否为生成的属性
    /// </summary>
    /// <param name="propertySymbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGeneratorProperty(this IPropertySymbol propertySymbol)
    {
        switch (propertySymbol.Name)
        {
            case "EqualityContract":
                switch (propertySymbol.Type.Name)
                {
                    case "Type":
                        return true; // 记录 record 关键字将生成的属性
                }
                break;
            case "global::MemoryPack.IFixedSizeMemoryPackable.Size":
            case "MemoryPack.IFixedSizeMemoryPackable.Size":
            case "IFixedSizeMemoryPackable.Size":
                switch (propertySymbol.Type.Name)
                {
                    case "Int32":
                        return true; // MemoryPack.MemoryPackableAttribute 生成的属性
                }
                break;
        }

        return false;
    }
}
