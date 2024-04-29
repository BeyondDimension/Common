namespace BD.Common8.Toast.Extensions;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 尝试添加 Toast 的服务依赖
    /// </summary>
    /// <typeparam name="TToastImpl"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IServiceCollection TryAddToast<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TToastImpl>(
        this IServiceCollection services) where TToastImpl : ToastBaseImpl
    {
        services.TryAddSingleton<TToastImpl>();
        services.TryAddSingleton<IToastIntercept, NoneToastIntercept>();
        services.TryAddSingleton<IToast>(static s => s.GetRequiredService<TToastImpl>());
        return services;
    }
}