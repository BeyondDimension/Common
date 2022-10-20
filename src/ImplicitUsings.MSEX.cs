// C# 10 定义全局 using

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
#if !BLAZOR && !__NOT_IMPORT_WEBENCODERS__
global using Microsoft.Extensions.WebEncoders;
#endif
global using Microsoft.Extensions.Primitives;
