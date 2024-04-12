//namespace BD.Common8.AspNetCore.Controllers;

///// <summary>
///// 通用测试输出环境信息接口控制器
///// </summary>
//public sealed partial class InfoController : AllowAnonymousApiController<InfoController>
//{
//    readonly IWebHostEnvironment env;
//    readonly IUserManager userManager;
//    readonly ApplicationDbContextBase db;
//    readonly IJWTValueProvider jwtValueProvider;
//    readonly BMAppSettings settings;
//    readonly ApplicationPartManager partManager;

//    public InfoController(
//        IWebHostEnvironment env,
//        ApplicationDbContextBase db,
//        IUserManager userManager,
//        IJWTValueProvider jwtValueProvider,
//        IOptions<BMAppSettings> options,
//        ApplicationPartManager partManager,
//        ILogger<InfoController> logger) : base(logger)
//    {
//        this.env = env;
//        this.db = db;
//        this.userManager = userManager;
//        this.jwtValueProvider = jwtValueProvider;
//        this.partManager = partManager;
//        settings = options.Value;
//    }

//    /// <summary>
//    /// 获取当前服务器系统信息
//    /// </summary>
//    /// <returns></returns>
//    [HttpGet]
//    public IActionResult Get()
//    {
//        if (env.IsDevelopment())
//        {
//            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
//            var now = DateTime.Now;
//            var timeZoneInfo = TimeZoneInfo.Local;
//            static Dictionary<string, dynamic> _CultureInfo(CultureInfo cultureInfo) => new()
//            {
//                { "LCID", cultureInfo.LCID },
//                { "Name", cultureInfo.Name },
//                { "NativeName", cultureInfo.NativeName },
//                { "DisplayName", cultureInfo.DisplayName },
//                { "EnglishName", cultureInfo.EnglishName },
//                { "TwoLetterISOLanguageName", cultureInfo.TwoLetterISOLanguageName },
//                { "ThreeLetterISOLanguageName", cultureInfo.ThreeLetterISOLanguageName },
//                { "ThreeLetterWindowsLanguageName", cultureInfo.ThreeLetterWindowsLanguageName },
//            };
//            static Dictionary<string, dynamic> CamelCase(Dictionary<string, dynamic> dict) => dict.ToDictionary(k => JsonNamingPolicy.CamelCase.ConvertName(k.Key), v => v.Value);
//            var controllerFeature = new ControllerFeature();
//            partManager.PopulateFeature(controllerFeature);
//            var tagHelperFeature = new TagHelperFeature();
//            partManager.PopulateFeature(tagHelperFeature);
//            var viewComponentFeature = new ViewComponentFeature();
//            partManager.PopulateFeature(viewComponentFeature);
//            return new JsonResult(new
//            {
//                IpAddress = ip,
//                ProgramHelper.ProjectName,
//                ProgramHelper.Version,
//                RuntimeVersion = Environment.Version,
//                CentralProcessorName = $"{ProgramHelper.CentralProcessorName} x{Environment.ProcessorCount}",
//                env.WebRootPath,
//                env.ContentRootPath,
//                env.EnvironmentName,
//                env.ApplicationName,
//                CurrentCulture = CamelCase(_CultureInfo(CultureInfo.CurrentCulture)),
//                CurrentUICulture = CamelCase(_CultureInfo(CultureInfo.CurrentUICulture)),
//                RawUrl = Request.RawUrl(),
//                Request.Protocol,
//                UserHostAddress = Request.UserHostAddress(),
//                UserAgent = Request.UserAgent(),
//                Request.Headers.AcceptLanguage,
//                Now = CamelCase(new()
//                {
//                    { "Default", now },
//                    { "RFC1123", now.ToString("r") },
//                    { "Standard", now.ToString("yyyy-MM-dd HH:mm:ss") },
//                }),
//                now.DayOfWeek,
//                TimeZoneInfo = CamelCase(new()
//                {
//                    { "Id", timeZoneInfo.Id },
//                    { "DisplayName", timeZoneInfo.DisplayName },
//                    { "StandardName", timeZoneInfo.StandardName },
//                    { "DaylightName", timeZoneInfo.DaylightName },
//                    { "BaseUtcOffset", timeZoneInfo.BaseUtcOffset },
//                }),
//                ApplicationParts = new
//                {
//                    Controllers = controllerFeature.Controllers.Select(x => x.Name),
//                    TagHelpers = tagHelperFeature.TagHelpers.Select(x => x.Name),
//                    ViewComponents = viewComponentFeature.ViewComponents.Select(x => x.Name),
//                },
//            });
//        }
//        return Content($"{ProgramHelper.ProjectName} v{ProgramHelper.Version}");
//    }
//}