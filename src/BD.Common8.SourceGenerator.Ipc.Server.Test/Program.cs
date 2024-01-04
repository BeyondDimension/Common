#pragma warning disable SA1600 // Elements should be documented

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

[ServiceContractImpl(typeof(ITodoService), IpcGeneratorType.Server)]
sealed partial class TodoServiceImpl : ITodoService
{
    public Task<ApiRspImpl<Todo[]?>> All(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Todo> AsyncEnumerable(int len, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ApiRspImpl> Exception1(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Todo> Exception2(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<ApiRspImpl<Todo>> Exception3(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ApiRspImpl<Todo?>> GetById(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ApiRspImpl> SimpleTypes(bool p0, byte p1, sbyte p2, char p3, DateOnly p4, DateTime p5, DateTimeOffset p6, decimal p7, double p8, ProcessorArchitecture p9, Guid p10, short p11, int p12, long p13, float p14, TimeOnly p15, TimeSpan p16, ushort p17, uint p18, ulong p19, Uri p20, Version p21, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    Task<ApiRspImpl> ITodoService.Tuple(bool p0, byte p1, sbyte p2, char p3, DateOnly p4, DateTime p5, DateTimeOffset p6, decimal p7, double p8, ProcessorArchitecture[] p9, Guid p10, short p11, int p12, long p13, float p14, TimeOnly p15, TimeSpan p16, ushort p17, uint p18, ulong[] p19, Uri p20, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}