#if NET35 || NET40
#pragma warning disable IDE0005 // Using 指令是不需要的。
global using MethodImplOptions = System.Runtime.CompilerServices.MethodImplOptionsEnum;

namespace System.Runtime.CompilerServices;

static partial class MethodImplOptionsEnum
{
    /// <summary>
    /// 指示此方法在非托管的代码中实现
    /// </summary>
    public const MethodImplOptions Unmanaged = (MethodImplOptions)4;

    /// <summary>
    /// 指示该方法不能为内联方法
    /// </summary>
    public const MethodImplOptions NoInlining = (MethodImplOptions)8;

    /// <summary>
    /// 指示已声明该方法，但在其他位置提供实现
    /// </summary>
    public const MethodImplOptions ForwardRef = (MethodImplOptions)16;

    /// <summary>
    /// 指示该方法一次性只能在一个线程上执行
    /// </summary>
    public const MethodImplOptions Synchronized = (MethodImplOptions)32;

    /// <summary>
    /// 指示调试可能的代码生成问题时，该方法不由实时 (JIT) 编译器或本机代码生成优化
    /// </summary>
    public const MethodImplOptions NoOptimization = (MethodImplOptions)64;

    /// <summary>
    /// 指示完全按照声明导出方法签名
    /// </summary>
    public const MethodImplOptions PreserveSig = (MethodImplOptions)128;

    /// <summary>
    /// 指示方法应进行积极的内联优化
    /// </summary>
    public const MethodImplOptions AggressiveInlining = (MethodImplOptions)256;

    /// <summary>
    /// 指示方法包含的代码应始终针对性能进行优化
    /// </summary>
    public const MethodImplOptions AggressiveOptimization = (MethodImplOptions)512;

    /// <summary>
    /// 指示方法为内部调用
    /// </summary>
    public const MethodImplOptions InternalCall = (MethodImplOptions)4096;
}
#endif