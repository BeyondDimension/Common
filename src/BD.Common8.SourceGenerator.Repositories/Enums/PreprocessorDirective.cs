namespace BD.Common8.SourceGenerator.Repositories.Enums;

/// <summary>
/// C# 预处理器指令
/// <para>https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/preprocessor-directives</para>
/// </summary>
public enum PreprocessorDirective
{
    #region 定义区域

    // https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/preprocessor-directives#defining-regions
    // 利用 #region，可以指定在使用代码编辑器的大纲功能时可展开或折叠的代码块。
    // 在较长的代码文件中，折叠或隐藏一个或多个区域十分便利，这样，可将精力集中于当前处理的文件部分。

    /// <summary>
    /// 启动区域
    /// </summary>
    region,

    /// <summary>
    /// 结束区域
    /// </summary>
    endregion,

    #endregion

    #region 条件编译

    /// <summary>
    /// 打开条件编译，其中仅在定义了指定的符号时才会编译代码。
    /// </summary>
    @if,

    /// <summary>
    /// 打开条件编译，其中仅在定义了指定的符号时才会编译代码。
    /// </summary>
    elif,

    /// <summary>
    /// 关闭前面的条件编译，如果没有定义前面指定的符号，打开一个新的条件编译。
    /// </summary>
    @else,

    /// <summary>
    /// 关闭前面的条件编译。
    /// </summary>
    endif,

    #endregion
}
