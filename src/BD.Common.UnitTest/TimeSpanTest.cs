namespace BD.Common.UnitTest;

public sealed class TimeSpanTest
{
    [Test]
    public void ToDisplayString()
    {
        TimeSpan timeSpan = TimeSpan.FromHours(1800);
        timeSpan = timeSpan.Add(TimeSpan.FromMinutes(35));
        timeSpan = timeSpan.Add(TimeSpan.FromSeconds(25));
        timeSpan = timeSpan.Add(TimeSpan.FromMicroseconds(125));

        var str = timeSpan.ToString();
        var str2 = $"{Math.Floor(timeSpan.TotalHours)}:{timeSpan.Minutes}:{timeSpan.Seconds}";
    }
}
