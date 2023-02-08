using NJObject = Newtonsoft.Json.Linq.JObject;
using SJObject = System.Text.Json.Nodes.JsonObject;

namespace BD.Common.UnitTest;

public sealed class JsonTest
{
    const string json0 = """
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

    [Test]
    public void DuplicateKey_NJson()
    {
        var obj = Serializable.DJSON<NJObject>(Serializable.JsonImplType.NewtonsoftJson, json0);
        TestContext.WriteLine(obj);
        var value = obj?["test1"]?.ToString();
        Assert.That(value, Is.EqualTo("😋"));
    }

    [Test]
    public void DuplicateKey_SJson()
    {
        try
        {
            var obj = Serializable.DJSON<SJObject>(Serializable.JsonImplType.SystemTextJson, json0);
            TestContext.WriteLine(obj);
            var value = obj?["test1"]?.ToString();
            Assert.That(value, Is.EqualTo("😋"));
        }
        catch (ArgumentException)
        {
            return;
        }

        /* 消息: 
    System.ArgumentException : An item with the same key has already been added. Key: test1 (Parameter 'propertyName')

  堆栈跟踪: 
    ThrowHelper.ThrowArgumentException_DuplicateKey(String paramName, String propertyName)
    JsonObject.InitializeIfRequired()
    JsonNode.get_Item(String propertyName)
         */

        throw new ApplicationException(
            "SystemTextJson 在重复键的情况下应该会抛出异常，所以需要 NewtonsoftJson");
    }
}
