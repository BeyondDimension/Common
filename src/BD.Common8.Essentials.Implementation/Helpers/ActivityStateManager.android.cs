#if ANDROID
using Application = Android.App.Application;
using Context = Android.Content.Context;

namespace BD.Common8.Essentials.Helpers;

/// <summary>
/// Represents a manager object that can handle <see cref="Activity"/> states.
/// <para>https://github.com/dotnet/maui/blob/8.0.0-rc.2.9373/src/Essentials/src/Platform/ActivityStateManager.android.cs</para>
/// </summary>
public static partial class ActivityStateManager
{
    static ActivityLifecycleContextListener? lifecycleListener;

    /// <summary>
    /// Gets the <see cref="Activity"/> object that represents the application's current activity.
    /// </summary>
    /// <param name="manager">The object to invoke this method on.</param>
    /// <param name="throwOnNull">Throws an exception if no current <see cref="Activity"/> can be found and this value is set to <see langword="true"/>, otherwise this method returns <see langword="null"/>.</param>
    /// <exception cref="NullReferenceException">Thrown if no current <see cref="Activity"/> can be found and <paramref name="throwOnNull"/> is set to <see langword="true"/>.</exception>
    [SupportedOSPlatform("android")]
    public static Activity? GetCurrentActivity([DoesNotReturnIf(true)] bool throwOnNull = false)
    {
        var activity = lifecycleListener?.Activity;
        if (throwOnNull && activity == null)
            throw new NullReferenceException("The current Activity can not be detected. Ensure that you have called Init in your Activity or Application class.");
        return activity;
    }

    /// <summary>
    /// Occurs when the state of an activity of this application changes.
    /// </summary>
    [SupportedOSPlatform("android")]
    public static event EventHandler<ActivityStateChangedEventArgs>? ActivityStateChanged;

    /// <summary>
    /// Initializes the <see cref="ActivityStateManager"/> for the given <see cref="Android.App.Application"/>.
    /// </summary>
    /// <param name="application">The <see cref="Android.App.Application"/> to use for initialization.</param>
    [SupportedOSPlatform("android")]
    public static void Init(Application application)
    {
        lifecycleListener = new ActivityLifecycleContextListener(OnActivityStateChanged);
        application.RegisterActivityLifecycleCallbacks(lifecycleListener);
    }

    /// <summary>
    /// Initializes the <see cref="ActivityStateManager"/> for the given <see cref="Activity"/> and <see cref="Bundle"/>.
    /// </summary>
    /// <param name="activity">The <see cref="Activity"/> to use for initialization.</param>
    [SupportedOSPlatform("android")]
    public static void Init(Activity activity)
    {
        if (activity.Application is not Application application)
            throw new InvalidOperationException("Activity was not attached to an application.");

        Init(application);
        lifecycleListener!.Activity = activity;
    }

    /// <summary>
    /// Waits for a <see cref="Activity"/> to be created or resumed.
    /// </summary>
    /// <param name="cancelToken">A token that can be used for cancelling the operation.</param>
    /// <returns>The application's current <see cref="Activity"/> or the <see cref="Activity"/> that has been created or resumed.</returns>
    [SupportedOSPlatform("android")]
    public static async Task<Activity> WaitForActivityAsync(CancellationToken cancelToken = default)
    {
        if (GetCurrentActivity() is Activity activity)
            return activity;

        var tcs = new TaskCompletionSource<Activity>();

        try
        {
            using (cancelToken.Register(() => tcs.TrySetCanceled()))
            {
                ActivityStateChanged += handler;
                return await tcs.Task.ConfigureAwait(false);
            }
        }
        finally
        {
            ActivityStateChanged -= handler;
        }

        void handler(object? sender, ActivityStateChangedEventArgs e)
        {
            if (e.State == ActivityState.Created || e.State == ActivityState.Resumed)
                tcs.TrySetResult(e.Activity);
        }
    }

    static void OnActivityStateChanged(Activity activity, ActivityState ev)
        => ActivityStateChanged?.Invoke(null, new ActivityStateChangedEventArgs(activity, ev));
}

/// <summary>
/// Represents states that a <see cref="Activity"/> can have.
/// </summary>
[SupportedOSPlatform("android")]
public enum ActivityState
{
    /// <summary>The activity is created.</summary>
    Created,

    /// <summary>The activity is resumed.</summary>
    Resumed,

    /// <summary>The activity is paused.</summary>
    Paused,

    /// <summary>The activity is destroyed.</summary>
    Destroyed,

    /// <summary>The activity saving the instance state.</summary>
    SaveInstanceState,

    /// <summary>The activity is started.</summary>
    Started,

    /// <summary>The activity is stopped.</summary>
    Stopped,
}

[SupportedOSPlatform("android")]
public sealed class ActivityStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityStateChangedEventArgs"/> class.
    /// </summary>
    /// <param name="activity"></param>
    /// <param name="ev"></param>
    internal ActivityStateChangedEventArgs(Activity activity, ActivityState ev)
    {
        State = ev;
        Activity = activity;
    }

    public ActivityState State { get; }

    public Activity Activity { get; }
}

[SupportedOSPlatform("android")]
sealed class ActivityLifecycleContextListener(Action<Activity, ActivityState> onActivityStateChanged) : Java.Lang.Object, Application.IActivityLifecycleCallbacks
{
    readonly Action<Activity, ActivityState> _onActivityStateChanged = onActivityStateChanged;
    readonly WeakReference<Activity?> _currentActivity = new(null);

    public Context Context => Activity ?? Application.Context;

    public Activity? Activity
    {
        get => _currentActivity.TryGetTarget(out var a) ? a : null;
        set => _currentActivity.SetTarget(value);
    }

    void Application.IActivityLifecycleCallbacks.OnActivityCreated(Activity activity, Bundle? savedInstanceState)
    {
        Activity = activity;
        _onActivityStateChanged(activity, ActivityState.Created);
    }

    void Application.IActivityLifecycleCallbacks.OnActivityDestroyed(Activity activity) =>
        _onActivityStateChanged(activity, ActivityState.Destroyed);

    void Application.IActivityLifecycleCallbacks.OnActivityPaused(Activity activity)
    {
        Activity = activity;
        _onActivityStateChanged(activity, ActivityState.Paused);
    }

    void Application.IActivityLifecycleCallbacks.OnActivityResumed(Activity activity)
    {
        Activity = activity;
        _onActivityStateChanged(activity, ActivityState.Resumed);
    }

    void Application.IActivityLifecycleCallbacks.OnActivitySaveInstanceState(Activity activity, Bundle outState) =>
        _onActivityStateChanged(activity, ActivityState.SaveInstanceState);

    void Application.IActivityLifecycleCallbacks.OnActivityStarted(Activity activity) =>
        _onActivityStateChanged(activity, ActivityState.Started);

    void Application.IActivityLifecycleCallbacks.OnActivityStopped(Activity activity) =>
        _onActivityStateChanged(activity, ActivityState.Stopped);
}
#endif