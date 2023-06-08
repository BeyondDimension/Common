namespace BD.Common.Repositories.SourceGenerator.Handlers.Properties.Abstractions;

/// <summary>
/// 属性处理
/// </summary>
public interface IPropertyHandle
{
    /// <summary>
    /// 自定义将属性元数据写入到源码文件流中的行为
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    bool Write(PropertyHandleArguments args);
}
