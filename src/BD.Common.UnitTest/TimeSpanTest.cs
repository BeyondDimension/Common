namespace BD.Common.UnitTest;

public sealed class TimeSpanTest
{
    [Test]
    public void ToDisplayString()
    {
        TimeSpan timeSpan = TimeSpan.FromHours(5);
        timeSpan = timeSpan.Add(TimeSpan.FromMinutes(6));
        timeSpan = timeSpan.Add(TimeSpan.FromSeconds(7));
        timeSpan = timeSpan.Add(TimeSpan.FromMicroseconds(125));

        var str = timeSpan.ToString();
        var str2 = $"{Math.Floor(timeSpan.TotalHours):00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        TestContext.WriteLine(str);
        TestContext.WriteLine(str2);
    }
}
