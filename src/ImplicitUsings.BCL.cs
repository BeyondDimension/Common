// C# 10 定义全局 using

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE0005
#pragma warning disable SA1209 // Using alias directives should be placed after other using directives
#pragma warning disable SA1211 // Using alias directives should be ordered alphabetically by alias name

global using Microsoft.Win32;
global using System.Collections.Concurrent;
global using System.Collections.ObjectModel;
#if !NETFRAMEWORK
#if !SOURCE_GENERATOR || __HAVE_S_JSON__
global using System.Collections.Immutable;
#endif
global using System.ComponentModel;
#if !SOURCE_GENERATOR
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
#endif
#endif
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.IO.Pipes;
global using System.IO.Compression;
#if !BLAZOR && !SOURCE_GENERATOR
global using System.IO.FileFormats;
#endif
global using System.Linq;
global using System.Linq.Expressions;
global using System.Web;
global using System.Net;
global using System.Net.Security;
global using System.Net.Http.Headers;
#if !SOURCE_GENERATOR
global using System.Net.Http.Json;
#endif
#if !BLAZOR && !SOURCE_GENERATOR
global using System.Net.Http.Client;
#endif
global using System.Net.Sockets;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Runtime.Serialization;
global using System.Security;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Security.Principal;
global using System.Text;
#if !SOURCE_GENERATOR
global using System.Text.Encodings.Web;
global using System.Text.Unicode;
#endif
#if !SOURCE_GENERATOR || __HAVE_S_JSON__
global using System.Text.Json;
global using System.Text.Json.Nodes;
global using System.Text.Json.Serialization;
global using System.Text.Json.Serialization.Metadata;
#endif
global using System.Text.RegularExpressions;
global using System.Runtime;
#if !BLAZOR && !SOURCE_GENERATOR
global using DeploymentMode = System.Runtime.DeploymentMode;
global using System.Runtime.Devices;
#endif
global using System.Runtime.InteropServices;
global using System.Runtime.Versioning;
global using System.Runtime.Serialization.Formatters;

#if !SOURCE_GENERATOR
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
#endif

#if WINDOWS7_0_OR_GREATER
global using WPFMessageBox = MS.Win32.MessageBox;
#endif

global using System.Security.Cryptography.X509Certificates;
global using IPAddress = System.Net.IPAddress;
#if !BLAZOR && !SOURCE_GENERATOR
global using Ioc = System.Ioc;
global using DateTimeFormat = System.DateTimeFormat;
#endif
global using SerializationDateTimeFormat = System.Runtime.Serialization.DateTimeFormat;