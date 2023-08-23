namespace System;

/// <summary>
/// https://github.com/CommunityToolkit/dotnet/blob/v8.0.0-preview3/CommunityToolkit.Mvvm/DependencyInjection/Ioc.cs
/// </summary>
public static partial class Ioc
{
#if !NOT_DI

    static volatile IServiceProvider? value;

    internal static bool IsConfigured => value != null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Dispose()
    {
        if (value is IDisposable disposable) disposable.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask DisposeAsync()
    {
        if (value is IAsyncDisposable disposable) await disposable.DisposeAsync();
        else Dispose();
    }

    /// <summary>
    /// 初始化依赖注入服务组(通过配置服务项的方式)
    /// </summary>
    /// <param name="configureServices"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ConfigureServices(IServiceProvider serviceProvider)
    {
        Interlocked.CompareExchange(ref value, serviceProvider, null);
    }

#endif

#if !NOT_DI

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Exception GetDIGetFailException(Type serviceType)
    {
        var msg = $"DI.Get* fail, serviceType: {serviceType}";
        Debug.WriteLine(msg);
        return new(msg);
    }

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    public static IServiceScope CreateScope()
    {
        if (value == null)
        {
            var msg = "DI.CreateScope fail.";
            Debug.WriteLine(msg);
            throw new Exception(msg);
        }
        return value.CreateScope();
    }

#endif
}

[Obsolete("use Ioc", true)]
public static partial class DI
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Dispose() => Ioc.Dispose();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask DisposeAsync() => Ioc.DisposeAsync();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ConfigureServices(Action<IServiceCollection> configureServices) => Ioc.ConfigureServices(configureServices);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ConfigureServices(IServiceProvider serviceProvider)
        => Ioc.ConfigureServices(serviceProvider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Get<T>() where T : notnull
        => Ioc.Get<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? Get_Nullable<T>() where T : notnull
        => Ioc.Get_Nullable<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Get(Type serviceType)
        => Ioc.Get(serviceType);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object? Get_Nullable(Type serviceType)
        => Ioc.Get_Nullable(serviceType);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IServiceScope CreateScope()
        => Ioc.CreateScope();
}