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
  "test3": false
}
""";

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
    }

    /// <summary>
    /// 重复键的测试，使用 <see cref="System.Text.Json"/>
    /// </summary>
    [Test]
    public void DuplicateKey_SJson()
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
  "test3": false
}
""";
        var obj = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.Nodes.JsonObject>(json0);
        //var obj = Serializable.DJSON<SystemTextJsonObject>(Serializable.JsonImplType.SystemTextJson, json0);
        TestContext.WriteLine(obj);
        var value = obj.GetValue(() =>
        {
            var value = obj?["test1"]?.ToString();
            return value;
        });
        Assert.That(value, Is.EqualTo("😋"));
    }
}
