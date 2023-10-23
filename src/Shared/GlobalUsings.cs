// C# 10 定义全局 using

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE0005 // 删除不必要的 using 指令
#pragma warning disable SA1209 // Using alias directives should be placed after other using directives
#pragma warning disable SA1211 // Using alias directives should be ordered alphabetically by alias name

global using Microsoft.Win32;
global using Microsoft.Win32.SafeHandles;
global using System.CodeDom.Compiler;
global using System.Collections;
#if (NETFRAMEWORK && NET40_OR_GREATER) || !NETFRAMEWORK
global using System.Collections.Concurrent;
#endif
global using System.Collections.ObjectModel;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.IO.Compression;
global using System.IO.Pipes;
global using System.Linq;
global using System.Linq.Expressions;
global using System.Net;
global using System.Net.NetworkInformation;
global using System.Net.Security;
global using System.Net.Sockets;
global using System.Reflection;
global using System.Resources;
global using System.Runtime;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Runtime.Serialization;
global using System.Runtime.Serialization.Formatters;
global using System.Runtime.Versioning;
global using System.Security;
#if (NETFRAMEWORK && NET45_OR_GREATER) || !NETFRAMEWORK
global using System.Security.Claims;
#endif
global using System.Security.Cryptography;
global using System.Security.Cryptography.X509Certificates;
global using System.Security.Principal;
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Web;
global using System.Xml;
global using System.Xml.Linq;
global using System.Xml.Serialization;
global using IPAddress = System.Net.IPAddress;

#if MACCATALYST || MACOS || IOS
global using ObjCRuntime;
global using Foundation;
global using CoreFoundation;
#endif