namespace Tools.Build.Commands;

partial interface IBuildCommand
{
    const string slnFileName_Common8 = "BD.Common8";
    const string slnFileName_Avalonia8 = "BD.Avalonia8";
    const string slnFileName_SteamClient8 = "BD.SteamClient8";
    const string slnFileName_SPPSDK = "SPP.SDK";

    const string jsonConfigFileName = "build-projects.json";

    static string[] GetSlnFileNames() =>
    [
        slnFileName_Common8,
        slnFileName_Avalonia8,
        slnFileName_SteamClient8,
        slnFileName_SPPSDK,
    ];

    static string[] GetProjectNamesCore(string slnFileName) => slnFileName switch
    {
        slnFileName_Common8 => [
                // 3.SourceGenerator
                "BD.Common8.SourceGenerator.ResX",
                //"BD.Common8.SourceGenerator.ResX.Test",
                "BD.Common8.SourceGenerator.Bcl",
                "BD.Common8.SourceGenerator.Bcl.Test",
                "BD.Common8.SourceGenerator.Ipc.Client",
                "BD.Common8.SourceGenerator.Ipc.Client.Test",
                "BD.Common8.SourceGenerator.Ipc.Server",
                "BD.Common8.SourceGenerator.Ipc.Server.Test",
                "BD.Common8.SourceGenerator.Repositories",
                "BD.Common8.SourceGenerator.Shared",

                // AspNetCore
                "BD.Common8.AspNetCore",
                "BD.Common8.AspNetCore.Identity",
                "BD.Common8.AspNetCore.Identity.BackManage",

                // Bcl
                "BD.Common8.Bcl",
                "BD.Common8.Bcl.Compat",

                // Crawler
                "BD.Common8.Crawler",

                // Essentials
                "BD.Common8.Essentials",
                "BD.Common8.Essentials.Implementation",
                "BD.Common8.Essentials.Implementation.Avalonia",

                // Http
                "BD.Common8.Http.ClientFactory",
                "BD.Common8.Http.ClientFactory.Server",

                // HuaweiCloud
                "BD.Common8.HuaweiCloud.SDK.Obs",

                // Ipc
                "BD.Common8.Ipc",
                "BD.Common8.Ipc.Client",
                "BD.Common8.Ipc.Server",

                // Orm
                "BD.Common8.Orm.EFCore",

                // Pinyin
                "BD.Common8.Pinyin",
                "BD.Common8.Pinyin.ChnCharInfo",
                "BD.Common8.Pinyin.CoreFoundation",

                // Primitives
                "BD.Common8.Primitives.ApiRsp",
                "BD.Common8.Primitives.ApiResponse",

                "BD.Common8.Primitives.PersonalData.BirthDate",
                "BD.Common8.Primitives.PersonalData.PhoneNumber",
                "BD.Common8.Primitives.PersonalData.UserName",

                "BD.Common8.Primitives",
                "BD.Common8.Primitives.District",
                "BD.Common8.Primitives.Essentials",
                "BD.Common8.Primitives.Toast",

                // Repositories
                "BD.Common8.Repositories",
                "BD.Common8.Repositories.EFCore",
                "BD.Common8.Repositories.SQLitePCL",

                // Security
                "BD.Common8.Security",

                // Settings
                "BD.Common8.Settings5",

                // Sms
                "BD.Common8.SmsSender",

                // Toast
                "BD.Common8.Toast",

                // UserInput
                "BD.Common8.UserInput.ModelValidator",
            ],
        slnFileName_Avalonia8 => [
                "BD.Avalonia8.Image2",
            ],
        slnFileName_SteamClient8 => [
                "BD.SteamClient8",
                "BD.SteamClient8",
                "BD.SteamClient8.Impl",
                "BD.SteamClient8.Models",
                "BD.SteamClient8.Primitives",
            ],
        slnFileName_SPPSDK => [
                "Mobius.Primitives",
                "Mobius.Models",
                "Mobius.SDK",
        ],
        _ => GetProjectNamesByJsonConfig(slnFileName),
    };

    static string[] GetProjectNamesByJsonConfig(string slnFileName)
    {
        var projPath = ROOT_ProjPath;
        if (File.Exists(Path.Combine(projPath, $"{slnFileName}.sln")))
        {
            var jsonConfigPath = Path.Combine(projPath, "src", jsonConfigFileName);
            if (File.Exists(jsonConfigPath))
            {
                using var stream = new FileStream(jsonConfigPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                var value = SystemTextJsonSerializer.Deserialize<string[]>(stream);
                if (value != null)
                {
                    return value;
                }
            }
        }
        return [];
    }
}