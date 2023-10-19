namespace BD.Common8.UnitTest;

/// <summary>
/// 提供对 <see cref="MachineUniqueIdentifier"/> 的单元测试
/// </summary>
public sealed class MachineUniqueIdentifierTest
{
    /// <summary>
    /// 测试获取 MachineSecretKey
    /// </summary>
    [Test]
    public void GetMachineSecretKey()
    {
        var machineSecretKey = MachineUniqueIdentifier.Value;
        TestContext.WriteLine("Key:");
        TestContext.WriteLine(machineSecretKey.Key.ToHexString());
        TestContext.WriteLine("IV:");
        TestContext.WriteLine(machineSecretKey.IV.ToHexString());
    }
}
