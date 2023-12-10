using static BD.Common8.UnitTest.Constants;
using HuaWeiCloudSmsSenderProvider = BD.Common8.SmsSender.Services.Implementation.SmsSender.Channels.HuaweiCloud.SmsSenderProvider;

namespace BD.Common8.UnitTest;

/// <summary>
/// 短信测试
/// </summary>
public sealed class SmsSenderTest
{
    static async Task GeneralTest(
        ISmsSender smsSender,
        string? message = null,
        bool generateRandomNum = true)
    {
        if (generateRandomNum)
        {
            message ??= string.Join(null, new char[Sms_length].Select(x => '6'));
        }

        var sendSmsResult = await smsSender.SendSmsAsync(sms_phone_number, message!, 0);
        Assert.That(sendSmsResult.IsSuccess);
        TestContext.WriteLine($"HttpStatusCode: {sendSmsResult.HttpStatusCode}");
        TestContext.WriteLine($"Record: {sendSmsResult.Result?.GetRecord()}");

        //var checkSmsResult = await smsSender.CheckSmsAsync(sms_phone_number, message!);
        //Assert.That(checkSmsResult.IsCheckSuccess);
    }

    /// <summary>
    /// 调试短信发送程序提供程序测试
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task DebugSmsSenderProviderTest()
    {
        ISmsSender smsSender = new DebugSmsSenderProvider();
        await GeneralTest(smsSender);
    }

    /// <summary>
    ///  华为云短信发送程序提供程序测试
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task HuaWeiCloudSmsSenderProviderTest()
    {
        if (!enable_HuaWeiCloudSmsSenderProviderTest)
            return;

        using var httpClient = new HttpClient();
        var loggerFactory = LoggerFactory.Create(static o => o.AddConsole());
        var logger = loggerFactory.CreateLogger<HuaWeiCloudSmsSenderProvider>();

        SmsHuaweiCloudOptions options = new()
        {
            ApiAddress = hwc_api_address,
            AppKey = hwc_app_key,
            AppSecret = hwc_app_secret,
            Signature = hwc_signature,
            Sender = hwc_sender,
            StatusCallBack = hwc_status_callback,
            DefaultTemplate = hwc_default_template,
        };
        ISmsSender smsSender = new HuaWeiCloudSmsSenderProvider(logger, options, httpClient);
        await GeneralTest(smsSender);
    }
}
