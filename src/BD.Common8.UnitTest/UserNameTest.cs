namespace BD.Common8.UnitTest;

/// <summary>
/// 用户名相关单元测试
/// </summary>
public sealed class UserNameTest
{
    [Test]
    public void WriteLine()
    {
        Enumerable.Range(0, byte.MaxValue).ForEach(i => TestContext.WriteLine(UserNameGenerateHelper.GenerateEnglishUserName()));
    }
}
