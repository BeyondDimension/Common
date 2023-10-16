namespace System;

/// <summary>
/// 异常已知类型，通常不为 <see cref="Unknown"/> 的异常不需要纪录日志
/// </summary>
public enum ExceptionKnownType : byte
{
    Unknown,

    /// <summary>
    /// 取消操作异常
    /// </summary>
    Canceled,

    /// <summary>
    /// 证书时间验证错误异常，通常为本地时间不正确导致 SSL 握手失败或服务端证书失效
    /// </summary>
    CertificateNotYetValid,

    /// <inheritdoc cref="OperationCanceledException"/>
    OperationCanceled,

    /// <inheritdoc cref="TaskCanceledException"/>
    TaskCanceled,
}