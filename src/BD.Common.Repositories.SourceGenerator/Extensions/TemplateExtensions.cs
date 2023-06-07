// ReSharper disable once CheckNamespace
namespace System;

public static class TemplateExtensions
{
    /// <summary>
    /// 从模板元数据中获取所有参数
    /// </summary>
    /// <typeparam name="TTemplateMetadata"></typeparam>
    /// <param name="templateMetadata"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object[] GetArgs<TTemplateMetadata>(this TTemplateMetadata templateMetadata) where TTemplateMetadata : ITemplateMetadata
    {
        var args = typeof(TTemplateMetadata).
            GetProperties(BindingFlags.Public | BindingFlags.Instance).
            Where(x => x.CanWrite && x.CanRead).
            Select(x => x.GetValue(templateMetadata)).
            ToArray();
        return args;
    }
}
