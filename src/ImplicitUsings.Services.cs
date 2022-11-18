// C# 10 定义全局 using

global using BD.Common.Services;
#if !__NOT_IMPORT_SERVICES_IMPL__
global using BD.Common.Services.Implementation;
#endif