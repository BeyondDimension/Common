namespace System;

/// <summary>
/// 线程安全的随机数生成
/// <para>https://devblogs.microsoft.com/pfxteam/getting-random-numbers-in-a-thread-safe-way</para>
/// </summary>
public static partial class Random2
{
#if !NET6_0_OR_GREATER
    [ThreadStatic]
    static Random? _local;

    /// <summary>
    /// 提供可从任何线程并发使用的线程安全 Random 实例
    /// </summary>
    public static Random Shared()
    {
        var inst = _local;
        if (inst == null)
        {
            //byte[] buffer = new byte[4];
            //_global.GetBytes(buffer);
            //_local = inst = new Random(BitConverter.ToInt32(buffer, 0));

            // GUID 生成随机数性能比 RNGCryptoServiceProvider 更好
            _local = inst = new Random(Guid.NewGuid().GetHashCode());
        }
        return inst;
    }

#else

    // https://github.com/dotnet/runtime/blob/v6.0.6/src/libraries/System.Private.CoreLib/src/System/Random.cs#L52
    // https://github.com/dotnet/runtime/blob/v6.0.6/src/libraries/System.Private.CoreLib/src/System/Random.cs#L220

    /// <inheritdoc cref="Random.Shared"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Random Shared() => Random.Shared;

#endif

    /// <inheritdoc cref="Random.Next()"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Next() => Shared().Next();

    /// <inheritdoc cref="Random.Next(int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Next(int maxValue) => Shared().Next(maxValue);

    /// <inheritdoc cref="Random.Next(int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Next(int minValue, int maxValue) => Shared().Next(minValue, maxValue);

    /// <inheritdoc cref="Random.NextDouble"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double NextDouble() => Shared().NextDouble();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static char RandomCharAt(string s, int index)
    {
        if (index == s.Length) index = 0;
        else if (index > s.Length) index %= s.Length;
        return s[index];
    }

    /// <summary>
    /// 生成随机字符串，长度为固定传入字符串
    /// </summary>
    /// <param name="length">要生成的字符串长度</param>
    /// <param name="randomChars">随机字符串字符集</param>
    /// <returns></returns>
    public static string GenerateRandomString(int length = 6,
        string randomChars = String2.DigitsLetters)
    {
        var random = Shared();
        var result = new char[length];
        if (random.Next(256) % 2 == 0)
            for (var i = length - 1; i >= 0; i--) // 5 4 3 2 1 0
                EachGenerate(i);
        else
            for (var i = 0; i < length; i++) // 0 1 2 3 4 5
                EachGenerate(i);
        return new string(result);
        void EachGenerate(int i)
        {
            var index = random.Next(0, randomChars.Length);
            var temp = RandomCharAt(randomChars, index);
            result[i] = temp;
        }
    }

    /// <summary>
    /// 生成随机数字，长度为固定传入参数
    /// </summary>
    /// <param name="length">要生成的字符串长度</param>
    /// <param name="endIsZero">生成的数字最后一位是否能够为0，默认不能为0(<see langword="false"/>)</param>
    /// <returns></returns>
    public static int GenerateRandomNum(int length = 6, bool endIsZero = false)
    {
        if (length > 11) length = 11;
        var random = Shared();
        var result = 0;
        var lastNum = 0;
        if (random.Next(256) % 2 == 0)
            for (int i = length - 1; i >= 0; i--) // 5 4 3 2 1 0
                EachGenerate(i);
        else
            for (int i = 0; i < length; i++) // 0 1 2 3 4 5
                EachGenerate(i);
        return result;
        void EachGenerate(int i)
        {
            var bit = (int)(i == 0 ? 1 : Math.Pow(10, i));
            // 100,000  10,000  1,000   100     10      1
            // 1        10      100     1,000   10,000  100,000
            var current = random.Next(lastNum + 1, lastNum + 10);
            lastNum = current % 10;
            if (lastNum == 0)
            {
                // i != 0 &&  i!=5 末尾和开头不能有零
                if ((i != 0 || endIsZero) && i != length - 1)
                    return;
                lastNum = random.Next(1, 10);
            }
            result += lastNum * bit;
        }
    }

    /// <summary>
    /// 从多个元素中随机获取一个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetRandomItem<T>(this IReadOnlyList<T> items) => items[Next(items.Count)];
}