//namespace BD.Common8.AspNetCore.Controllers.Infrastructure;

///// <summary>
///// Initializes a new instance of the <see cref="TestController"/> class.
///// </summary>
///// <param name="logger"></param>
//[Route("test")]
//public sealed class TestController(ILogger<TestController> logger) : BaseAuthorizeController<TestController>(logger)
//{
//    [HttpGet("1")]
//    public async Task<ApiResponse> T1()
//    {
//        await Task.Delay(0);
//        logger.LogDebug("Test Exception");
//        throw new Exception("Test Exception");
//    }

//    [HttpGet("2")]
//    public Task<ApiResponse> T2() => T1();
//}
