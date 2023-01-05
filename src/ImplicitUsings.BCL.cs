// C# 10 定义全局 using

global using Microsoft.Win32;
#if !NETFRAMEWORK
global using System.Collections.Immutable;
global using System.ComponentModel;
global using System.ComponentModel.DataAnnotations;
#endif
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.IO.Compression;
global using System.Linq;
global using System.Linq.Expressions;
global using System.Net;
global using System.Net.Http.Json;
global using System.Net.Sockets;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Runtime.Serialization;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;
global using System.Text.Encodings.Web;
global using System.Text.Json;
global using System.Text.Json.Serialization;
#if !BLAZOR && !__API_RSP__
global using System.Runtime;
global using System.Runtime.Devices;
#endif
global using System.Runtime.InteropServices;
global using System.Runtime.Versioning;

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;

global using BD.Common;