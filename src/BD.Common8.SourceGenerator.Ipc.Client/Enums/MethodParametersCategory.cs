namespace BD.Common8.SourceGenerator.Ipc.Enums;

/// <summary>
/// 方法参数种类
/// </summary>
public enum MethodParametersCategory : byte
{
    /// <summary>
    /// 未知，忽略此类型
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// 无参数函数
    /// </summary>
    None,

    /// <summary>
    /// 参数全部为简单类型，使用路由
    /// </summary>
    SimpleTypes,

    /// <summary>
    /// 单个参数作为模型类，从 Body 中传递
    /// </summary>
    FromBody,
}

public static partial class MethodParametersCategoryEnumExtensions
{
    /// <summary>
    /// 根据 <see cref="IMethodSymbol"/> 解析 <see cref="MethodParametersCategory"/>
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static MethodParametersCategory GetMethodParametersCategory(this IMethodSymbol method)
    {
        if (method.Parameters.Length == 0) // 无参数函数
            return MethodParametersCategory.None;
        else if (method.Parameters.All(x => x.Type.IsSimpleTypes())) // 参数全部为简单类型，使用路由
            return MethodParametersCategory.SimpleTypes;
        else if (method.Parameters.Length == 1)
            return MethodParametersCategory.FromBody;

        return MethodParametersCategory.Unknown;
    }

    /// <summary>
    /// 根据 <see cref="MethodParametersCategory"/> 与 <see cref="IParameterSymbol"/> 获取参数类型字符串
    /// </summary>
    /// <param name="category"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
#pragma warning disable IDE0060 // 删除未使用的参数
    public static string GetParameterTypeString(this MethodParametersCategory category, IParameterSymbol parameter)
#pragma warning restore IDE0060 // 删除未使用的参数
    {
        var typeString = parameter.Type.ToDisplayString();
        if (typeString.Count(static x => x == '.') == 1)
        {
            typeString = typeString.TrimStart("System.");
        }
        return typeString;
    }
}