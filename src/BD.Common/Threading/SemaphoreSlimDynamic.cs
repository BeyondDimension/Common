// Copyright (c) Microsoft Corporation.
// Licensed under the BSD-3-Clause.
// https://github.com/Polly-Contrib/Polly.Contrib.MutableBulkheadPolicy/blob/ad2e3cc77808ca1ba08539df9aa282797329dfab/src/Polly.Contrib.MutableBulkheadPolicy/SemaphoreSlimDynamic.cs

namespace System.Threading;

/// <summary>
/// Compose <see cref="SemaphoreSlim" /> to allow to dynamically increase and
/// decrease the number of threads that can access a resource or pool of resources concurrently.
/// </summary>
/// <seealso cref="SemaphoreSlim" />
[DebuggerDisplay("Current Count = {CurrentCount}")]
public class SemaphoreSlimDynamic : IDisposable
{
    readonly object _lock;

    readonly SemaphoreSlim _semaphore;

    int _currAvailableSlotsCount;

    /// <summary>
    /// Gets the minimum number of slots.
    /// </summary>
    /// <value>
    /// The minimum slots count.
    /// </value>
    public int MinimumSlotsCount { get; }

    /// <summary>
    /// Gets the number of slots currently available.
    /// </summary>
    /// <value>
    /// The available slots count.
    /// </value>
    public int AvailableSlotsCount { get; private set; }

    /// <summary>
    /// Gets the maximum number of slots.
    /// </summary>
    /// <value>
    /// The maximum slots count.
    /// </value>
    public int MaximumSlotsCount { get; }

    /// <summary>
    /// Gets the current count of the <see cref="SemaphoreSlimDynamic"/>.
    /// </summary>
    /// <value>The current count of the <see cref="SemaphoreSlimDynamic"/>.</value>
    public int CurrentCount => _semaphore.CurrentCount;

    /// <summary>
    /// Returns a <see cref="T:System.Threading.WaitHandle"/> that can be used to wait on the semaphore.
    /// </summary>
    /// <value>A <see cref="T:System.Threading.WaitHandle"/> that can be used to wait on the
    /// semaphore.</value>
    /// <remarks>
    /// A successful wait on the <see cref="AvailableWaitHandle"/> does not imply a successful wait on
    /// the <see cref="SemaphoreSlimDynamic"/> itself, nor does it decrement the semaphore's
    /// count. <see cref="AvailableWaitHandle"/> exists to allow a thread to block waiting on multiple
    /// semaphores, but such a wait should be followed by a true wait on the target semaphore.
    /// </remarks>
    /// <exception cref="T:System.ObjectDisposedException">The <see
    /// cref="SemaphoreSlimDynamic"/> has been disposed.</exception>
    public WaitHandle AvailableWaitHandle => _semaphore.AvailableWaitHandle;

    /// <summary>
    /// Initializes a new instance of the <see cref="SemaphoreSlimDynamic"/> class.
    /// </summary>
    /// <param name="initialCount">The initial number of slots.</param>
    /// <param name="maxCount">The maximum number of slots.</param>
    public SemaphoreSlimDynamic(int initialCount, int maxCount) : this(0, initialCount, maxCount)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SemaphoreSlimDynamic"/> class.
    /// </summary>
    /// <param name="minCount">The minimum number of slots.</param>
    /// <param name="initialCount">The initial number of slots.</param>
    /// <param name="maxCount">The maximum number of slots.</param>
    public SemaphoreSlimDynamic(int minCount, int initialCount, int maxCount)
    {
        if (minCount < 0 || minCount > initialCount)
        {
            throw new ArgumentOutOfRangeException(nameof(minCount));
        }
        else if (initialCount > maxCount)
        {
            throw new ArgumentOutOfRangeException(nameof(initialCount));
        }

        _lock = new object();

        MinimumSlotsCount = minCount;
        AvailableSlotsCount = _currAvailableSlotsCount = initialCount;
        MaximumSlotsCount = maxCount;

        _semaphore = new SemaphoreSlim(initialCount, maxCount);
    }

    /// <summary>
    /// Attempts to change the number of slots
    /// </summary>
    /// <param name="newAvailableSlot">The number of available slots to change to.</param>
    /// <returns></returns>
    public bool SetAvailableSlot(int newAvailableSlot)
    {
        // Early exit when newAvailableSlot is out of range or no change.
        if (newAvailableSlot < MinimumSlotsCount || newAvailableSlot > MaximumSlotsCount)
        {
            throw new ArgumentOutOfRangeException(nameof(newAvailableSlot));
        }
        else if (newAvailableSlot == AvailableSlotsCount)
        {
            return false;
        }

        var changed = false;

        lock (_lock)
        {
            // Set available spot even if it failed to wait on decrease.
            // Extra logic in Release() will ensure extra capacity is taken away when released.
            AvailableSlotsCount = newAvailableSlot;

            try
            {
                int delta = newAvailableSlot - _currAvailableSlotsCount;

                if (delta > 0)
                {
                    for (int i = 0; i < delta; i++)
                    {
                        if (_currAvailableSlotsCount < MaximumSlotsCount)
                        {
                            _semaphore.Release();
                            _currAvailableSlotsCount++;
                            changed = true;
                        }
                    }
                }
                else if (delta < 0)
                {
                    for (int i = 0; i < Math.Abs(delta); i++)
                    {
                        if (_currAvailableSlotsCount > MinimumSlotsCount)
                        {
                            // Try to reduce capacity as much as possible. Do not block.
                            // The strategy is to skip on future releases until _currAvailableSlotsCount matches AvailableSlotsCount
                            if (_semaphore.Wait(TimeSpan.Zero))
                            {
                                _currAvailableSlotsCount--;
                                changed = true;
                            }
                        }
                    }
                }
            }
            catch (SemaphoreFullException)
            {
                // An exception is thrown if we attempt to exceed the max number of concurrent tasks
                // This could happen when AvailableSlotsCount is changed with positive delta
                // while some other tasks called Release().
                //
                // It's safe to ignore this exception.
            }
        }

        return changed;
    }

    /// <summary>
    /// Blocks the current thread until it can enter the <see cref="SemaphoreSlimDynamic"/>.
    /// </summary>
    /// <exception cref="T:System.ObjectDisposedException">The current instance has already been
    /// disposed.</exception>
    public void Wait()
    {
        // Call wait with infinite timeout
        _semaphore.Wait();
    }

    /// <summary>
    /// Blocks the current thread until it can enter the <see cref="SemaphoreSlimDynamic"/>, using a <see
    /// cref="T:System.TimeSpan"/> to measure the time interval.
    /// </summary>
    /// <param name="timeout">A <see cref="System.TimeSpan"/> that represents the number of milliseconds
    /// to wait, or a <see cref="System.TimeSpan"/> that represents -1 milliseconds to wait indefinitely.
    /// </param>
    /// <returns>true if the current thread successfully entered the <see cref="SemaphoreSlimDynamic"/>;
    /// otherwise, false.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="timeout"/> is a negative
    /// number other than -1 milliseconds, which represents an infinite time-out -or- timeout is greater
    /// than <see cref="int.MaxValue"/>.</exception>
    public bool Wait(TimeSpan timeout)
    {
        return _semaphore.Wait(timeout);
    }

    /// <summary>
    /// Blocks the current thread until it can enter the <see cref="SemaphoreSlimDynamic"/>, while observing a
    /// <see cref="T:System.Threading.CancellationToken"/>.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken"/> token to
    /// observe.</param>
    /// <exception cref="T:System.OperationCanceledException"><paramref name="cancellationToken"/> was
    /// canceled.</exception>
    /// <exception cref="T:System.ObjectDisposedException">The current instance has already been
    /// disposed.</exception>
    public void Wait(CancellationToken cancellationToken)
    {
        _semaphore.Wait(cancellationToken);
    }

    /// <summary>
    /// Blocks the current thread until it can enter the <see cref="SemaphoreSlimDynamic"/>, using a <see
    /// cref="T:System.TimeSpan"/> to measure the time interval, while observing a <see
    /// cref="T:System.Threading.CancellationToken"/>.
    /// </summary>
    /// <param name="timeout">A <see cref="System.TimeSpan"/> that represents the number of milliseconds
    /// to wait, or a <see cref="System.TimeSpan"/> that represents -1 milliseconds to wait indefinitely.
    /// </param>
    /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken"/> to
    /// observe.</param>
    /// <returns>true if the current thread successfully entered the <see cref="SemaphoreSlimDynamic"/>;
    /// otherwise, false.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="timeout"/> is a negative
    /// number other than -1 milliseconds, which represents an infinite time-out -or- timeout is greater
    /// than <see cref="int.MaxValue"/>.</exception>
    /// <exception cref="System.OperationCanceledException"><paramref name="cancellationToken"/> was canceled.</exception>
    public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
    {
        return _semaphore.Wait(timeout, cancellationToken);
    }

    /// <summary>
    /// Blocks the current thread until it can enter the <see cref="SemaphoreSlimDynamic"/>, using a 32-bit
    /// signed integer to measure the time interval.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see
    /// cref="System.Threading.Timeout.Infinite"/>(-1) to wait indefinitely.</param>
    /// <returns>true if the current thread successfully entered the <see cref="SemaphoreSlimDynamic"/>;
    /// otherwise, false.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="millisecondsTimeout"/> is a
    /// negative number other than -1, which represents an infinite time-out.</exception>
    public bool Wait(int millisecondsTimeout)
    {
        return _semaphore.Wait(millisecondsTimeout);
    }

    /// <summary>
    /// Asynchronously waits to enter the <see cref="SemaphoreSlimDynamic"/>.
    /// </summary>
    /// <returns>A task that will complete when the semaphore has been entered.</returns>
    public Task WaitAsync()
    {
        return _semaphore.WaitAsync();
    }

    /// <summary>
    /// Asynchronously waits to enter the <see cref="SemaphoreSlimDynamic"/>, while observing a
    /// <see cref="T:System.Threading.CancellationToken"/>.
    /// </summary>
    /// <returns>A task that will complete when the semaphore has been entered.</returns>
    /// <param name="cancellationToken">
    /// The <see cref="T:System.Threading.CancellationToken"/> token to observe.
    /// </param>
    /// <exception cref="T:System.ObjectDisposedException">
    /// The current instance has already been disposed.
    /// </exception>
    public Task WaitAsync(CancellationToken cancellationToken)
    {
        return _semaphore.WaitAsync(cancellationToken);
    }

    /// <summary>
    /// Asynchronously waits to enter the <see cref="SemaphoreSlimDynamic"/>,
    /// using a 32-bit signed integer to measure the time interval.
    /// </summary>
    /// <param name="millisecondsTimeout">
    /// The number of milliseconds to wait, or <see cref="T:System.Threading:Timeout.Infinite"/>(-1) to wait indefinitely.
    /// </param>
    /// <returns>
    /// A task that will complete with a result of true if the current thread successfully entered 
    /// the <see cref="SemaphoreSlimDynamic"/>, otherwise with a result of false.
    /// </returns>
    /// <exception cref="T:System.ObjectDisposedException">The current instance has already been
    /// disposed.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="millisecondsTimeout"/> is a negative number other than -1,
    /// which represents an infinite time-out.
    /// </exception>
    public Task<bool> WaitAsync(int millisecondsTimeout)
    {
        return _semaphore.WaitAsync(millisecondsTimeout);
    }

    /// <summary>
    /// Asynchronously waits to enter the <see cref="SemaphoreSlimDynamic"/>, using a <see
    /// cref="T:System.TimeSpan"/> to measure the time interval, while observing a
    /// <see cref="T:System.Threading.CancellationToken"/>.
    /// </summary>
    /// <param name="timeout">
    /// A <see cref="System.TimeSpan"/> that represents the number of milliseconds
    /// to wait, or a <see cref="System.TimeSpan"/> that represents -1 milliseconds to wait indefinitely.
    /// </param>
    /// <returns>
    /// A task that will complete with a result of true if the current thread successfully entered 
    /// the <see cref="SemaphoreSlimDynamic"/>, otherwise with a result of false.
    /// </returns>
    /// <exception cref="T:System.ObjectDisposedException">
    /// The current instance has already been disposed.
    /// </exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is a negative number other than -1 milliseconds, which represents 
    /// an infinite time-out -or- timeout is greater than <see cref="int.MaxValue"/>.
    /// </exception>
    public Task<bool> WaitAsync(TimeSpan timeout)
    {
        return _semaphore.WaitAsync(timeout);
    }

    /// <summary>
    /// Asynchronously waits to enter the <see cref="SemaphoreSlimDynamic"/>, using a <see
    /// cref="T:System.TimeSpan"/> to measure the time interval.
    /// </summary>
    /// <param name="timeout">
    /// A <see cref="System.TimeSpan"/> that represents the number of milliseconds
    /// to wait, or a <see cref="System.TimeSpan"/> that represents -1 milliseconds to wait indefinitely.
    /// </param>
    /// <param name="cancellationToken">
    /// The <see cref="T:System.Threading.CancellationToken"/> token to observe.
    /// </param>
    /// <returns>
    /// A task that will complete with a result of true if the current thread successfully entered 
    /// the <see cref="SemaphoreSlimDynamic"/>, otherwise with a result of false.
    /// </returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="timeout"/> is a negative number other than -1 milliseconds, which represents 
    /// an infinite time-out -or- timeout is greater than <see cref="int.MaxValue"/>.
    /// </exception>
    public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        return _semaphore.WaitAsync(timeout, cancellationToken);
    }

    /// <summary>
    /// Exits the <see cref="SemaphoreSlimDynamic"/> once.
    /// </summary>
    /// <returns>The previous count of the <see cref="SemaphoreSlimDynamic"/>.</returns>
    /// <exception cref="T:System.ObjectDisposedException">The current instance has already been
    /// disposed.</exception>
    public int Release()
    {
        lock (_lock)
        {
            // When reducing available slot, it is possible there are more tasks in semaphore than available.
            // This will prevent the non-blocking Wait inside AvailableSlotsCount setter to fail.
            // We could skip Release from user and eventually match _currAvailableSlotsCount to AvailableSlotsCount.
            if (_currAvailableSlotsCount == AvailableSlotsCount)
            {
                return _semaphore.Release();
            }
            else if (_currAvailableSlotsCount > AvailableSlotsCount)
            {
                // Hold back on the release
                _currAvailableSlotsCount--;
                return _semaphore.CurrentCount;
            }
            else // if (this._currAvailableSlotsCount < this.AvailableSlotsCount)
            {
                _currAvailableSlotsCount++;
                return _semaphore.Release();
            }
        }
    }

    /// <summary>
    /// Exits the <see cref="SemaphoreSlimDynamic"/> a specified number of times.
    /// </summary>
    /// <param name="releaseCount">The number of times to exit the semaphore.</param>
    /// <returns>The previous count of the <see cref="SemaphoreSlimDynamic"/>.</returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="releaseCount"/> is less
    /// than 1.</exception>
    /// <exception cref="T:System.Threading.SemaphoreFullException">The <see cref="SemaphoreSlimDynamic"/> has
    /// already reached its maximum size.</exception>
    /// <exception cref="T:System.ObjectDisposedException">The current instance has already been
    /// disposed.</exception>
    public int Release(int releaseCount)
    {
        lock (_lock)
        {
            // When reducing available slot, it is possible there are more tasks in semaphore than available.
            //
            // Example: if there are 5 available slots and all are occupied with Waits,
            // setting available slots to 3 should not allow further non-blocking Waits until more than 2 slots are Released.
            //
            // We could skip Release from user and eventually match _currAvailableSlotsCount to AvailableSlotsCount.
            if (_currAvailableSlotsCount == AvailableSlotsCount)
            {
                return _semaphore.Release(releaseCount);
            }
            else if (_currAvailableSlotsCount > AvailableSlotsCount)
            {
                // Hold back on the releases until avaiable slot matches.
                int delta = _currAvailableSlotsCount - AvailableSlotsCount;
                _currAvailableSlotsCount -= Math.Min(releaseCount, delta);

                // Release whatever the slots after available slots aligns.
                return (releaseCount > delta) ? _semaphore.Release(releaseCount - delta) : _semaphore.CurrentCount;
            }
            else // if (this._currAvailableSlotsCount < this.AvailableSlotsCount)
            {
                int delta = AvailableSlotsCount - _currAvailableSlotsCount;
                _currAvailableSlotsCount += Math.Min(releaseCount, delta);
                return (releaseCount > 0) ? _semaphore.Release(releaseCount) : _semaphore.CurrentCount;
            }
        }
    }

    public void Dispose()
    {
        _semaphore.Dispose();
    }
}