namespace System.Threading.Tasks;

/// <inheritdoc cref="Task"/>
public static partial class Task2
{
    /// <summary>
    /// 在后台线程中执行委托
    /// </summary>
    /// <param name="action"></param>
    /// <param name="longRunning"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async void InBackground(Action action, bool longRunning = false)
    {
        TaskCreationOptions options = TaskCreationOptions.DenyChildAttach;

        if (longRunning)
        {
            options |= TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness;
        }

        await Task.Factory.StartNew(action, CancellationToken.None, options, TaskScheduler.Default).ConfigureAwait(false);
    }

    /// <summary>
    /// 在后台线程中执行委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="function"></param>
    /// <param name="longRunning"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InBackground<T>(Func<T> function, bool longRunning = false)
    {
        InBackground(void () => function(), longRunning);
    }

    /// <summary>
    /// 并行化执行多个 <see cref="Task{TResult}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tasks"></param>
    /// <param name="minMemoryUsage"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<IList<T>> InParallel<T>(
        IEnumerable<Task<T>> tasks,
        bool minMemoryUsage = false)
    {
        if (minMemoryUsage)
        {
            var results = new List<T>();
            foreach (Task<T> task in tasks)
            {
                results.Add(await task.ConfigureAwait(false));
            }
            return results;
        }
        else
        {
            var results = await Task.WhenAll(tasks).ConfigureAwait(false);
            return results;
        }
    }

    /// <summary>
    /// 并行化执行多个 <see cref="Task"/>
    /// </summary>
    /// <param name="tasks"></param>
    /// <param name="minMemoryUsage"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task InParallel(
        IEnumerable<Task> tasks,
        bool minMemoryUsage = false)
    {
        if (minMemoryUsage)
        {
            foreach (Task task in tasks)
            {
                await task.ConfigureAwait(false);
            }
        }
        else
        {
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}
