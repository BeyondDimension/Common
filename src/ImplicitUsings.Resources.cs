// C# 10 定义全局 using

#if BASE_CLASS_LIB_EX
global using SR = System.Resources.Strings;
#else
global using SR = BD.Common.Resources.Strings;
#endif
