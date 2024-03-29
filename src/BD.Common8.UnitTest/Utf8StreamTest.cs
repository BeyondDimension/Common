namespace BD.Common8.UnitTest;

/// <summary>
/// 提供对 Utf8Stream 的单元测试
/// </summary>
public sealed class Utf8StreamTest
{
    /// <summary>
    /// 测试 Format，仅 1 个参数
    /// </summary>
    [Test]
    public void Format_args()
    {
        var test_01 = "{0}";
        (var byString, var byStream) = TestWriteFormat(test_01, nameof(test_01));
        Assert.That(byString, Is.EqualTo(byStream));
    }

    /// <summary>
    /// 测试 Format，包含 1 个参数
    /// </summary>
    [Test]
    public void Format_args1()
    {
        var test_01 = "test format {0} aaa";
        (var byString, var byStream) = TestWriteFormat(test_01, nameof(test_01));
        Assert.That(byString, Is.EqualTo(byStream));
    }

    /// <summary>
    /// 测试 Format，包含 2 个参数
    /// </summary>
    [Test]
    public void Format_args2()
    {
        var test_01 = "test format {0} aaa {1}.";
        (var byString, var byStream) = TestWriteFormat(test_01,
            nameof(test_01),
            Environment.TickCount);
        Assert.That(byString, Is.EqualTo(byStream));
    }

    /// <summary>
    /// 测试 Format，包含 3 个参数
    /// </summary>
    [Test]
    public void Format_args3()
    {
        var test_01 = "test format {0} aaa {1}. $#!^DSAFDSA {2} {1}{0}";
        (var byString, var byStream) = TestWriteFormat(test_01,
            nameof(test_01),
            Environment.TickCount,
            DateTime.Now);
        Assert.That(byString, Is.EqualTo(byStream));
    }

    //[Test]
    //public void Format_irregular_args1()
    //{
    //    var test_01 = "test format {{0}} a{0}aa";
    //    (string byString, string byStream) = TestWriteFormat(test_01, nameof(test_01));
    //    Assert.That(byString, Is.EqualTo(byStream));
    //}

    //[Test]
    //public void Format_irregular_args2()
    //{
    //    var test_01 = "test form{{1}}at {{0}} aaa {{1}} {0} {{0}}.";
    //    (string byString, string byStream) = TestWriteFormat(test_01,
    //        nameof(test_01),
    //        Environment.TickCount);
    //    Assert.That(byString, Is.EqualTo(byStream));
    //}

    /// <summary>
    /// 测试 Format，模板为 C# 源码
    /// </summary>
    [Test]
    public void Format_Source_1()
    {
        var test_01 =
"""
// ReSharper disable once CheckNamespace
namespace {0}.Entities;

/// <summary>
/// {1}
/// </summary>
[Table("{2}")]
public sealed partial class {3}
{a

""";
        (var byString, var byStream) = TestWriteFormat2(test_01.Replace("{a", "{{"), test_01.Replace("{a", "{"),
            "BD.Common8.UnitTest",
            "Test x",
            "Utf8StreamTests",
            "Utf8StreamTest");
        Assert.That(byString, Is.EqualTo(byStream));
    }

    static (string byString, string byStream) TestWriteFormat(string format, params object?[] args)
    {
        var byString = string.Format(format, args);
        using var stream = new MemoryStream();
        stream.WriteFormat(Encoding.UTF8.GetBytes(format).AsSpan(), args);
        var byStream = Encoding.UTF8.GetString(stream.ToArray());
        TestContext.WriteLine($"byString: {byString}");
        TestContext.WriteLine($"byStream: {byStream}");
        return (byString, byStream);
    }

    static (string byString, string byStream) TestWriteFormat2(string format1, string format2, params object?[] args)
    {
        var byString = string.Format(format1, args);
        using var stream = new MemoryStream();
        stream.WriteFormat(Encoding.UTF8.GetBytes(format2).AsSpan(), args);
        var byStream = Encoding.UTF8.GetString(stream.ToArray());
        TestContext.WriteLine($"byString: {byString}");
        TestContext.WriteLine($"byStream: {byStream}");
        return (byString, byStream);
    }
}
