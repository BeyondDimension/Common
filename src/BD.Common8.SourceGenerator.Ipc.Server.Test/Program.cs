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

    /// <inheritdoc cref="IEndpointRouteMapGroup.OnMapGroup(IEndpointRouteBuilder)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void OnMapGroup(IEndpointRouteBuilder endpoints)
    {
        var builder = endpoints.MapGroup("/ITodoService").RequireAuthorization();
        builder.MapGet("/All", (Delegate)(static async (HttpContext ctx) => await Ioc.Get<ITodoService>().All(ctx.RequestAborted)));
        builder.MapGet("/GetById/{id}", (Delegate)(static async (HttpContext ctx, [FromRoute] int id) => await Ioc.Get<ITodoService>().GetById(id, ctx.RequestAborted)));
        builder.MapGet("/SimpleTypes/{p0}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}/{p11}/{p12}/{p13}/{p14}/{p15}/{p16}/{p17}/{p18}/{p19}/{p20}/{p21}", (Delegate)(static async (HttpContext ctx, [FromRoute] bool p0, [FromRoute] byte p1, [FromRoute] sbyte p2, [FromRoute] char p3, [FromRoute] DateOnly p4, [FromRoute] DateTime p5, [FromRoute] DateTimeOffset p6, [FromRoute] decimal p7, [FromRoute] double p8, [FromRoute] ProcessorArchitecture p9, [FromRoute] Guid p10, [FromRoute] short p11, [FromRoute] int p12, [FromRoute] long p13, [FromRoute] float p14, [FromRoute] TimeOnly p15, [FromRoute] TimeSpan p16, [FromRoute] ushort p17, [FromRoute] uint p18, [FromRoute] ulong p19, [FromRoute] Uri p20, [FromRoute] Version p21) => await Ioc.Get<ITodoService>().SimpleTypes(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, ctx.RequestAborted)));
        builder.MapPost("/BodyTest", (Delegate)(static async (HttpContext ctx, [FromBody] Tuple<Todo> body) => await Ioc.Get<ITodoService>().BodyTest(todo, ctx.RequestAborted)));
        builder.MapGet("/AsyncEnumerable/{len}", (Delegate)(static (HttpContext ctx, [FromRoute] int len) => Ioc.Get<ITodoService>().AsyncEnumerable(len, ctx.RequestAborted)));
        builder.MapPost("/Tuple", (Delegate)(static async (HttpContext ctx, [FromBody] Tuple<bool, byte, sbyte, char, DateOnly, DateTime, DateTimeOffset, Tuple<decimal, double, ProcessorArchitecture[], Guid, short, int, long, Tuple<float, TimeOnly, TimeSpan, ushort, uint, ulong[], Uri>>> body) => await Ioc.Get<ITodoService>().Tuple(body.Item1, body.Item2, body.Item3, body.Item4, body.Item5, body.Item6, body.Item7, body.Rest.Item1, body.Rest.Item2, body.Rest.Item3, body.Rest.Item4, body.Rest.Item5, body.Rest.Item6, body.Rest.Item7, body.Rest.Rest.Item1, body.Rest.Rest.Item2, body.Rest.Rest.Item3, body.Rest.Rest.Item4, body.Rest.Rest.Item5, body.Rest.Rest.Item6, body.Rest.Rest.Item7, ctx.RequestAborted)));
    }
}