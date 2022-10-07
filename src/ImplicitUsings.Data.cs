// C# 10 定义全局 using

global using BD.Common.Data;
#if !__NOT_IMPORT_DATA_ABSTRACTIONS__
global using BD.Common.Data.Abstractions;
#endif