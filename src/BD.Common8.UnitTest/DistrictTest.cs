namespace BD.Common8.UnitTest;

/// <summary>
/// 提供对 <see cref="District"/> 的单元测试
/// </summary>
public sealed class DistrictTest
{
    /// <summary>
    /// 测试获取所有的 <see cref="District"/> 数据
    /// </summary>
    [Test]
    public void GetAll()
    {
        var all = District.All;

        TestContext.WriteLine(all.GetRandomItem());
        TestContext.WriteLine(all.GetRandomItem());
        TestContext.WriteLine(all.GetRandomItem());
    }
}
