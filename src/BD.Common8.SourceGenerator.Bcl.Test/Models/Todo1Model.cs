using System.Runtime.CompilerServices;

namespace BD.Common8.SourceGenerator.Bcl.Test.Models;

/// <summary>
/// 测试 CopyPropertiesGeneratedAttribute 的服务类示例
/// </summary>
[CopyPropertiesGenerated]
[CopyPropertiesGenerated(destType: typeof(Todo2Model),
      IsExpression = true,
      IgnoreProperties = ["UsageSteamUserId"],
      MethodName = "P2SExpression",
      AppointProperties = "{\"UpdateTime\": \"UpdateTime ?? DateTimeOffset.Now\"}",
      MapProperties = "{\"UpdateTime\": \"UpdateTime2\"}")]
[CopyPropertiesGenerated(destType: typeof(Todo2Model),
      AppointProperties = "{\"CreationTime\": \"CreationTime ?? DateTimeOffset.Now\"}",
      OnlyProperties = ["CreationTime"])]
public partial class Todo1Model
{
    public Guid Id { get; set; }

    public DateTimeOffset? UpdateTime { get; set; }

    public DateTimeOffset? CreationTime { get; set; }

    public Guid? OperatorUserId { get; set; }

    public Guid? CreateUserId { get; set; }

    public Guid? UsageSteamUserId { get; set; }
}
