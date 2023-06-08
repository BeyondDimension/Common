namespace BD.Common.Repositories.SourceGenerator.Handlers.Attributes.Abstractions;

/// <summary>
/// 特性处理
/// </summary>
public interface IAttributeHandle
{
    /// <summary>
    /// 将特性写入到源码文件流中，如果已写入，则返回特性类型完整名称
    /// </summary>
    /// <param name="args"></param>
    string? Write(AttributeHandleArguments args);
}
