#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Ipc.Sample;
#pragma warning restore IDE0079 // 请删除不必要的忽略

#pragma warning disable SA1600 // Elements should be documented

[ServiceContract]
public partial interface ITodoService
{
    record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

    Task<ApiRspImpl<Todo[]?>> All();

    Task<ApiRspImpl<Todo?>> GetById(int id);

    /// <summary>
    /// 测试 https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-8.0#simple-types 路由绑定简单类型
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="p4"></param>
    /// <param name="p5"></param>
    /// <param name="p6"></param>
    /// <param name="p7"></param>
    /// <param name="p8"></param>
    /// <param name="p9"></param>
    /// <param name="p10"></param>
    /// <param name="p11"></param>
    /// <param name="p12"></param>
    /// <param name="p13"></param>
    /// <param name="p14"></param>
    /// <param name="p15"></param>
    /// <param name="p16"></param>
    /// <param name="p17"></param>
    /// <param name="p18"></param>
    /// <param name="p19"></param>
    /// <param name="p20"></param>
    /// <param name="p21"></param>
    /// <returns></returns>
    Task<ApiRspImpl> SimpleTypes(bool p0, byte p1, sbyte p2, char p3, DateOnly p4, DateTime p5, DateTimeOffset p6, decimal p7, double p8, ProcessorArchitecture p9, Guid p10, short p11, int p12, long p13, float p14, TimeOnly p15, TimeSpan p16, ushort p17, uint p18, ulong p19, Uri p20, Version p21);

    Task<ApiRspImpl> BodyTest(Todo todo)
    {
        throw new NotImplementedException();
    }
}
