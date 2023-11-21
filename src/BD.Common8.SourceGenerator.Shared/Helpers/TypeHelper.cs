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
}
