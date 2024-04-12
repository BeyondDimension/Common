namespace BD.Common8.AspNetCore.Controllers;

/// <summary>
/// IP 地址测试接口
/// </summary>
[AllowAnonymous]
[Route("api/[controller]/[action]")]
public sealed class IPController : ControllerBase
{
    /// <summary>
    /// 测试 IPv6 获取
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult V6()
    {
        var ip = HttpContext.Connection.RemoteIpAddress;
        if (ip != null && ip.AddressFamily == AddressFamily.InterNetworkV6)
        {
            return Ok();
        }
        return BadRequest();
    }

    /// <summary>
    /// 测试 IP 地址获取
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Val()
    {
        var ip = HttpContext.Connection.RemoteIpAddress;
        return Content(ip?.ToString() ?? "");
    }
}