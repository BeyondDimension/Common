namespace BD.Common8.UnitTest;

/// <summary>
/// 提供对 Json 的单元测试
/// </summary>
public sealed class JsonTest
{
    const string json0 =
"""
{
  "test1": "Hello World",
  "test1": "2",
  "test1": "#",
  "test1": "saf",
  "test1": "😋",
  "test2": 22,
  "test3": false,
  "test4": {
    "test1": "a",
    "test1": 3,
  }
}
""";

    SystemTextJsonSerializerOptions allowTrailingCommasOptions = null!;

    /// <inheritdoc cref="SetUpAttribute"/>
    [SetUp]
    public void Setup()
    {
        allowTrailingCommasOptions = new(SystemTextJsonSerializerOptions.Default)
        {
            AllowTrailingCommas = true,
        };
    }

    /// <summary>
    /// 重复键的测试，使用 <see cref="Newtonsoft.Json"/>
    /// </summary>
    [Test]
    public void DuplicateKey_NJson()
    {
        var obj = Serializable.DJSON<NewtonsoftJsonObject>(Serializable.JsonImplType.NewtonsoftJson, json0);
        TestContext.WriteLine(obj);
        var value = obj?["test1"]?.ToString();
        Assert.That(value, Is.EqualTo("😋"));
        var value2 = obj?["test4"]?["test1"]?.ToString();
        Assert.That(value2, Is.EqualTo("3"));
    }

    /// <summary>
    /// 重复键的测试，使用 <see cref="System.Text.Json"/>
    /// </summary>
    [Test]
    public void DuplicateKey_SJson()
    {
        var obj = SystemTextJsonSerializer.Deserialize<SystemTextJsonObject>(json0, allowTrailingCommasOptions);
        TestContext.WriteLine(obj);
        var value = obj.GetItem("test1")?.ToString();
        Assert.That(value, Is.EqualTo("😋"));
        var value2 = obj.GetItem("test4").GetItem("test1")?.ToString();
        Assert.That(value2, Is.EqualTo("3"));
    }
}
