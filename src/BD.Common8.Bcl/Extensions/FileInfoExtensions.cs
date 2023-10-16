namespace System.Extensions;

/// <summary>
/// 提供对 <see cref="FileInfo"/> 类型的扩展函数
/// </summary>
public static partial class FileInfoExtensions
{
    /// <summary>
    /// 根据 <see cref="FileInfo"/> 打开文本只读流，可选择编码，默认使用 UTF8 编码
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StreamReader? OpenText(this FileInfo fileInfo, Encoding? encoding = null)
    {
        var f = IOPath.OpenRead(fileInfo.FullName);
        if (f == null)
            return null;
        return new StreamReader(f, encoding ?? Encoding2.UTF8NoBOM);
    }
}