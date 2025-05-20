/*----------------------------------------------------------------------------------
// Copyright 2019 Huawei Technologies Co.,Ltd.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License.  You may obtain a copy of the
// License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations under the License.
//----------------------------------------------------------------------------------*/

using System.Runtime.CompilerServices;

namespace OBS.Internal.Log;

public partial interface ILoggerMgr
{
    bool IsDebugEnabled { get; }

    bool IsInfoEnabled { get; }

    bool IsWarnEnabled { get; }

    bool IsErrorEnabled { get; }

    void Debug(string param, Exception? exception);

    void Error(string param, Exception? exception);

    void Info(string param, Exception? exception);

    void Warn(string param, Exception? exception);
}

#if NETFRAMEWORK
public static partial class ILoggerMgr2 // fix CS8701 目标运行时不支持默认接口实现
#else
partial interface ILoggerMgr
#endif
{
    private static ILoggerMgr? loggerMgr;

    internal static ILoggerMgr Instance => loggerMgr ??= EmptyLoggerMgr.Instance;

    public static void Initialize(ILoggerMgr loggerMgr)
    {
#if NETFRAMEWORK
        ILoggerMgr2.loggerMgr = loggerMgr;
#else
        ILoggerMgr.loggerMgr = loggerMgr;
#endif
    }
}

sealed class EmptyLoggerMgr : ILoggerMgr
{
    public static readonly EmptyLoggerMgr Instance = new();

    EmptyLoggerMgr() { }

    public bool IsDebugEnabled => default;

    public bool IsInfoEnabled => default;

    public bool IsWarnEnabled => default;

    public bool IsErrorEnabled => default;

    public void Debug(string param, Exception? exception)
    {
    }

    public void Error(string param, Exception? exception)
    {
    }

    public void Info(string param, Exception? exception)
    {
    }

    public void Warn(string param, Exception? exception)
    {
    }
}

static class LoggerMgr
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ILoggerMgr GetInstance() =>
#if NETFRAMEWORK
        ILoggerMgr2.Instance;
#else
        ILoggerMgr.Instance;
#endif

    internal static bool IsDebugEnabled => GetInstance().IsDebugEnabled;

    internal static bool IsInfoEnabled => GetInstance().IsInfoEnabled;

    internal static bool IsWarnEnabled => GetInstance().IsWarnEnabled;

    internal static bool IsErrorEnabled => GetInstance().IsErrorEnabled;

    internal static void Debug(string param)
    {
        //_logger.Debug(param);
        Debug(param, null);
    }

    internal static void Error(string param)
    {
        //_logger.Error(param);
        Error(param, null);
    }

    internal static void Info(string param)
    {
        //_logger.Info(param);
        Info(param, null);
    }

    internal static void Warn(string param)
    {
        //_logger.Warn(param);
        Warn(param, null);
    }

    internal static void Debug(string param, Exception? exception) => GetInstance().Debug(param, exception);

    internal static void Error(string param, Exception? exception) => GetInstance().Error(param, exception);

    internal static void Info(string param, Exception? exception) => GetInstance().Info(param, exception);

    internal static void Warn(string param, Exception? exception) => GetInstance().Warn(param, exception);
}
