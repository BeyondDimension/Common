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

namespace OBS.Internal.Log;

public interface ILoggerMgr
{
    private static ILoggerMgr? loggerMgr;

    internal static ILoggerMgr Instance => loggerMgr ??= EmptyLoggerMgr.Instance;

    static void Initialize(ILoggerMgr loggerMgr)
    {
        ILoggerMgr.loggerMgr = loggerMgr;
    }

    bool IsDebugEnabled { get; }

    bool IsInfoEnabled { get; }

    bool IsWarnEnabled { get; }

    bool IsErrorEnabled { get; }

    void Debug(string param, Exception? exception);

    void Error(string param, Exception? exception);

    void Info(string param, Exception? exception);

    void Warn(string param, Exception? exception);
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
    internal static bool IsDebugEnabled => ILoggerMgr.Instance.IsDebugEnabled;

    internal static bool IsInfoEnabled => ILoggerMgr.Instance.IsInfoEnabled;

    internal static bool IsWarnEnabled => ILoggerMgr.Instance.IsWarnEnabled;

    internal static bool IsErrorEnabled => ILoggerMgr.Instance.IsErrorEnabled;

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

    internal static void Debug(string param, Exception? exception) => ILoggerMgr.Instance.Debug(param, exception);

    internal static void Error(string param, Exception? exception) => ILoggerMgr.Instance.Error(param, exception);

    internal static void Info(string param, Exception? exception) => ILoggerMgr.Instance.Info(param, exception);

    internal static void Warn(string param, Exception? exception) => ILoggerMgr.Instance.Warn(param, exception);
}
