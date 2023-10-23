// C# 10 定义全局 using

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE0005 // 删除不必要的 using 指令
#pragma warning disable SA1209 // Using alias directives should be placed after other using directives
#pragma warning disable SA1211 // Using alias directives should be ordered alphabetically by alias name

global using BD.Common8.SmsSender.Models.SmsSender;
global using BD.Common8.SmsSender.Models.SmsSender.Abstractions;

global using BD.Common8.SmsSender.Services;
global using BD.Common8.SmsSender.Services.Implementation.SmsSender;

global using BD.Common8.SmsSender.Models.SmsSender.Channels._21VianetBlueCloud;
global using BD.Common8.SmsSender.Models.SmsSender.Channels.AlibabaCloud;
global using BD.Common8.SmsSender.Models.SmsSender.Channels.NetEaseCloud;

global using BD.Common8.SmsSender.Services.Implementation.SmsSender.Channels._21VianetBlueCloud;
global using BD.Common8.SmsSender.Services.Implementation.SmsSender.Channels.AlibabaCloud;
global using BD.Common8.SmsSender.Services.Implementation.SmsSender.Channels.NetEaseCloud;