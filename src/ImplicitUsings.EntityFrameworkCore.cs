// C# 10 定义全局 using

global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Infrastructure;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
#if !__NOT_IMPORT_Z_EF_PLUS__
global using Z.EntityFramework.Plus;
#endif