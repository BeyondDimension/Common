#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加短信发送提供商(仅单选一个提供商)
    /// </summary>
    /// <typeparam name="TSmsSettings"></typeparam>
    /// <param name="services"></param>
    /// <param name="name">提供商唯一名称，见 <see cref="ISmsSettings.SmsOptions"/> 中 PropertyName</param>
    /// <returns></returns>
    public static IServiceCollection AddSmsSenderProvider<TSmsSettings>(
        this IServiceCollection services,
        string? name)
        where TSmsSettings : class, ISmsSettings
        => SmsSenderBase.Add<TSmsSettings>(services, name);
}