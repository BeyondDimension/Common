#if !NETFRAMEWORK || (NETSTANDARD && NETSTANDARD2_0_OR_GREATER)
// https://github.com/CommunityToolkit/dotnet/blob/v8.2.1/src/CommunityToolkit.Mvvm/DependencyInjection/Ioc.cs

namespace System;

/// <summary>
/// A type that facilitates the use of the <see cref="IServiceProvider"/> type.
/// The <see cref="Ioc"/> provides the ability to configure services in a singleton, thread-safe
/// service provider instance, which can then be used to resolve service instances.
/// The first step to use this feature is to declare some services, for instance:
/// <code>
/// public interface ILogger
/// {
///     void Log(string text);
/// }
/// </code>
/// <code>
/// public class ConsoleLogger : ILogger
/// {
///     void Log(string text) => Console.WriteLine(text);
/// }
/// </code>
/// Then the services configuration should then be done at startup, by calling the <see cref="ConfigureServices(IServiceProvider)"/>
/// method and passing an <see cref="IServiceProvider"/> instance with the services to use. That instance can
/// be from any library offering dependency injection functionality, such as Microsoft.Extensions.DependencyInjection.
/// For instance, using that library, <see cref="ConfigureServices(IServiceProvider)"/> can be used as follows in this example:
/// <code>
/// Ioc.Default.ConfigureServices(
///     new ServiceCollection()
///     .AddSingleton&lt;ILogger, Logger&gt;()
///     .BuildServiceProvider());
/// </code>
/// Finally, you can use the <see cref="Ioc"/> instance (which implements <see cref="IServiceProvider"/>)
/// to retrieve the service instances from anywhere in your application, by doing as follows:
/// <code>
/// Ioc.Default.GetService&lt;ILogger&gt;().Log("Hello world!");
/// </code>
/// </summary>
public static partial class Ioc
{
    static volatile IServiceProvider? value;

    /// <summary>
    /// 是否已配置 Ioc
    /// </summary>
    public static bool IsConfigured => value != null;

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public static void Dispose()
    {
        if (value is IDisposable disposable) disposable.Dispose();
    }

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
    public static async ValueTask DisposeAsync()
    {
        if (value is IAsyncDisposable disposable) await disposable.DisposeAsync();
        else Dispose();
    }

    /// <summary>
    /// 初始化依赖注入服务组(通过配置服务项的方式)
    /// </summary>
    /// <param name="configureServices"></param>
    public static void ConfigureServices(Action<IServiceCollection> configureServices)
    {
        var services = new ServiceCollection();
        configureServices(services);
        ConfigureServices(services.BuildServiceProvider());
    }

    /// <summary>
    /// 初始化依赖注入服务组(直接赋值)
    /// </summary>
    /// <param name="serviceProvider"></param>
    public static void ConfigureServices(IServiceProvider serviceProvider)
    {
        Interlocked.CompareExchange(ref value, serviceProvider, null);
    }

    static Exception GetDIGetFailException(Type serviceType)
    {
        var msg = $"DI.Get* fail, serviceType: {serviceType}";
        Debug.WriteLine(msg);
        return new(msg);
    }

    /// <inheritdoc cref="Fallback"/>
    public delegate object? FallbackDelegate(Type serviceType, bool required);

    /// <summary>
    /// 设置获取依赖注入服务自定义回退实现
    /// </summary>
    public static FallbackDelegate? Fallback { private get; set; }

    /// <summary>
    /// 获取依赖注入服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Get<T>() where T : notnull
    {
        if (value == null)
        {
            if (Fallback != null)
            {
                if (Fallback(typeof(T), true) is T t)
                {
                    return t;
                }
            }
            throw GetDIGetFailException(typeof(T));
        }
        try
        {
            return value.GetRequiredService<T>();
        }
        catch
        {
            if (Fallback != null)
            {
                if (Fallback(typeof(T), true) is T t)
                {
                    return t;
                }
            }
            throw;
        }
    }

    /// <inheritdoc cref="Get{T}"/>
    public static T? Get_Nullable<T>() where T : notnull
    {
        if (value == null)
        {
            if (Fallback != null)
            {
                if (Fallback(typeof(T), false) is T t)
                {
                    return t;
                }
            }
            return default;
        }
        T? r = default;
        try
        {
            r = value.GetService<T>();
        }
        catch (ObjectDisposedException)
        {
            return default;
        }
        catch
        {
            if (Fallback != null)
            {
                if (Fallback(typeof(T), false) is T t)
                {
                    return t;
                }
            }
            throw;
        }
        if (r is null)
        {
            if (Fallback != null)
            {
                if (Fallback(typeof(T), false) is T t)
                {
                    return t;
                }
            }
        }
        return r;
    }

    /// <inheritdoc cref="Get{T}"/>
    public static object Get(Type serviceType)
    {
        if (value == null)
        {
            if (Fallback != null)
            {
                var t = Fallback(serviceType, true);
                if (t != null)
                {
                    return t;
                }
            }
            throw GetDIGetFailException(serviceType);
        }
        try
        {
            return value.GetRequiredService(serviceType);
        }
        catch
        {
            if (Fallback != null)
            {
                var t = Fallback(serviceType, true);
                if (t != null)
                {
                    return t;
                }
            }
            throw;
        }
    }

    /// <inheritdoc cref="Get_Nullable{T}"/>
    public static object? Get_Nullable(Type serviceType)
    {
        if (value == null)
        {
            if (Fallback != null)
            {
                var t = Fallback(serviceType, false);
                if (t != null)
                {
                    return t;
                }
            }
            return default;
        }
        object? r = default;
        try
        {
            r = value.GetService(serviceType);
        }
        catch (ObjectDisposedException)
        {
            return default;
        }
        catch
        {
            if (Fallback != null)
            {
                var t = Fallback(serviceType, false);
                if (t != null)
                {
                    return t;
                }
            }
            throw;
        }
        if (r is null)
        {
            if (Fallback != null)
            {
                var t = Fallback(serviceType, false);
                if (t != null)
                {
                    return t;
                }
            }
        }
        return r;
    }

    /// <inheritdoc cref="ServiceProviderServiceExtensions.CreateScope(IServiceProvider)"/>
    public static IServiceScope CreateScope()
    {
        if (value == null)
        {
            var msg = "DI.CreateScope fail";
            Debug.WriteLine(msg);
            throw new Exception(msg);
        }
        return value.CreateScope();
    }
}
#endif