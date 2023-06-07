namespace BD.Common.UnitTest;

public sealed class Utf8StreamTest
{
    [Test]
    public void Format_args1()
    {
        var test_01 = "test format {0} aaa";
        (string byString, string byStream) = TestWriteFormat(test_01, nameof(test_01));
        Assert.That(byString, Is.EqualTo(byStream));
    }

    [Test]
    public void Format_args2()
    {
        var test_01 = "test format {0} aaa {1}.";
        (string byString, string byStream) = TestWriteFormat(test_01,
            nameof(test_01),
            Environment.TickCount);
        Assert.That(byString, Is.EqualTo(byStream));
    }

    [Test]
    public void Format_args3()
    {
        var test_01 = "test format {0} aaa {1}. $#!^DSAFDSA {2} {1}{0}";
        (string byString, string byStream) = TestWriteFormat(test_01,
            nameof(test_01),
            Environment.TickCount,
            DateTime.Now);
        Assert.That(byString, Is.EqualTo(byStream));
    }

    [Test]
    public void Format_irregular_args1()
    {
        var test_01 = "test format {{0}} a{0}aa";
        (string byString, string byStream) = TestWriteFormat(test_01, nameof(test_01));
        Assert.That(byString, Is.EqualTo(byStream));
    }

    [Test]
    public void Format_irregular_args2()
    {
        var test_01 = "test form{{1}}at {{0}} aaa {{1}} {0} {{0}}.";
        (string byString, string byStream) = TestWriteFormat(test_01,
            nameof(test_01),
            Environment.TickCount);
        Assert.That(byString, Is.EqualTo(byStream));
    }

    static (string byString, string byStream) TestWriteFormat(string format, params object?[] args)
    {
        var byString = string.Format(format, args);
        using var stream = new MemoryStream();
        stream.WriteFormat(Encoding.UTF8.GetBytes(byString).AsSpan(), args);
        var byStream = Encoding.UTF8.GetString(stream.ToArray());
        TestContext.WriteLine($"byString: {byString}");
        TestContext.WriteLine($"byStream: {byStream}");
        return (byString, byStream);
    }
}
