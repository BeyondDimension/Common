namespace BD.Common8.UnitTest;

/// <summary>
/// 提供对 <see cref="TimeSpan"/> 的单元测试
/// </summary>
public sealed class TimeSpanTest
{
    /// <summary>
    /// 测试显示字符串
    /// </summary>
    [Test]
    public void ToDisplayString()
    {
        var timeSpan = TimeSpan.FromHours(5);
        timeSpan = timeSpan.Add(TimeSpan.FromMinutes(6));
        timeSpan = timeSpan.Add(TimeSpan.FromSeconds(7));
        timeSpan = timeSpan.Add(TimeSpan.FromMicroseconds(125));

        var str = timeSpan.ToString();
        var str2 = $"{Math.Floor(timeSpan.TotalHours):00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        TestContext.WriteLine(str);
        TestContext.WriteLine(str2);
    }
}
