namespace BD.Common8.SourceGenerator.Helpers;

#pragma warning disable SA1600 // Elements should be documented

public static partial class TypeHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEnum(this ITypeSymbol typeSymbol) => typeSymbol.TypeKind == TypeKind.Enum;

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
}
