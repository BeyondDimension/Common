#if NET35 || NET40
#pragma warning disable IDE0005 // Using 指令是不需要的。
#pragma warning disable SA1600 // Elements should be documented
global using MethodImplOptions = System.Runtime.CompilerServices.MethodImplOptionsEnum;

namespace System.Runtime.CompilerServices;

static partial class MethodImplOptionsEnum
{
    public const MethodImplOptions Unmanaged = (MethodImplOptions)4;
    public const MethodImplOptions NoInlining = (MethodImplOptions)8;
    public const MethodImplOptions ForwardRef = (MethodImplOptions)16;
    public const MethodImplOptions Synchronized = (MethodImplOptions)32;
    public const MethodImplOptions NoOptimization = (MethodImplOptions)64;
    public const MethodImplOptions PreserveSig = (MethodImplOptions)128;
    public const MethodImplOptions AggressiveInlining = (MethodImplOptions)256;
    public const MethodImplOptions AggressiveOptimization = (MethodImplOptions)512;
    public const MethodImplOptions InternalCall = (MethodImplOptions)4096;
}
#endif