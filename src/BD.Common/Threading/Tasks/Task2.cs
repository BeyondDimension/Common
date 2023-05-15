namespace System.Threading.Tasks;

public static class Task2
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async void InBackground(Action action, bool longRunning = false)
    {
        ArgumentNullException.ThrowIfNull(action);

        TaskCreationOptions options = TaskCreationOptions.DenyChildAttach;

        if (longRunning)
        {
            options |= TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness;
        }

        await Task.Factory.StartNew(action, CancellationToken.None, options, TaskScheduler.Default).ConfigureAwait(false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InBackground<T>(Func<T> function, bool longRunning = false)
    {
        ArgumentNullException.ThrowIfNull(function);

        InBackground(void () => function(), longRunning);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<IList<T>> InParallel<T>(
        IEnumerable<Task<T>> tasks,
        bool minMemoryUsage = false)
    {
        ArgumentNullException.ThrowIfNull(tasks);

        IList<T> results;

        if (minMemoryUsage)
        {
            results = new List<T>();

            foreach (Task<T> task in tasks)
            {
                results.Add(await task.ConfigureAwait(false));
            }
        }
        else
        {
            results = await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        return results;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task InParallel(
        IEnumerable<Task> tasks,
        bool minMemoryUsage = false)
    {
        ArgumentNullException.ThrowIfNull(tasks);

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
