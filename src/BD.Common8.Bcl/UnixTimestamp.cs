namespace System;

public static partial class UnixTimestamp
{
    /// <summary>
    /// 计算给定 <see cref="DateTime"/> 相对于 <see cref="UnixEpochTicks"/> 的时间差
    /// </summary>
    /// <returns>转换后的 Unix 时间戳值</returns>
    static double ToUnixTimestamp(this DateTime dt, UnixTimestampType unixTimestampType)
    {
        var timeDiff = new TimeSpan(dt.ToUniversalTime().Ticks - UnixEpochTicks);
        var total = unixTimestampType switch
        {
            UnixTimestampType.Milliseconds => timeDiff.TotalMilliseconds,
            UnixTimestampType.Seconds => timeDiff.TotalSeconds,
            _ => throw new ArgumentOutOfRangeException(nameof(unixTimestampType), unixTimestampType, null),
        };
        return (double)Math.Floor(total);
    }

    /// <summary>
    /// 获取 Unix 时间戳转换为 <see cref="DateTime"/> 结构中的 Ticks 值
    /// </summary>
    static long GetTicks(long timestamp, UnixTimestampType unixTimestampType)
    {
        long ticks;
        switch (unixTimestampType)
        {
            case UnixTimestampType.Milliseconds:
                if (timestamp > TimestampMillisecondsMaxValue) timestamp = TimestampMillisecondsMaxValue;
                ticks = UnixEpochTicks + TimeSpan.FromMilliseconds(timestamp).Ticks;
                break;
            case UnixTimestampType.Seconds:
                ticks = UnixEpochTicks + TimeSpan.FromSeconds(timestamp).Ticks;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(unixTimestampType), unixTimestampType, null);
        }
        return ticks;
    }

    /// <summary>
    /// 将指定的 Ticks 数转换为对应的本地时间或者 UTC 时间的 <see cref="DateTime"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static DateTime GetDateTime(long ticks, bool convertLocalTime = true)
    {
        var dt = new DateTime(ticks, DateTimeKind.Utc);
        return convertLocalTime ? dt.ToLocalTime() : dt;
    }

    /// <summary>
    /// 将指定的 Ticks 数转换为对应的本地时间或者 UTC 时间的 <see cref="DateTimeOffset"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static DateTimeOffset GetDateTimeOffset(long ticks, bool convertLocalTime = true)
    {
        var dt = new DateTimeOffset(ticks, TimeSpan.Zero);
        return convertLocalTime ? dt.ToLocalTime() : dt;
    }
}