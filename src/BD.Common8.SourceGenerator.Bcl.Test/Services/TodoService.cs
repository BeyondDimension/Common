using System.Runtime.CompilerServices;

namespace BD.Common8.SourceGenerator.Bcl.Test.Services;

/// <summary>
/// 测试 SingletonPartitionGeneratedAttribute 的服务类示例
/// </summary>
[SingletonPartitionGenerated]
partial class TodoService
{
    public bool IsRunning { get; set; }

    public string? ServiceName { get; set; }

    public int ServiceId { get; set; }

    public Guid ServiceGuid { get; set; }

    public decimal Money { get; set; }

    public void StartService()
    {
        IsRunning = true;
    }

    public static void Start()
    {
        Current.StartService();
    }
}