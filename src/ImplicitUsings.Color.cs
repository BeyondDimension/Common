// C# 10 定义全局 using

global using GdiPlusColor = System.Drawing.Color;
#if MAUI
global using MauiColor = Microsoft.Maui.Graphics.Color;
#endif