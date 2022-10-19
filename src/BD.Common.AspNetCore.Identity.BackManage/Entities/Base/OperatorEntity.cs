using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD.Common.Entities;

public abstract class OperatorEntity<TPrimaryKey> :
    CreationEntity<TPrimaryKey>,
    IUpdateTime,
    IOperatorUser,
    IOperatorUserId
    where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTimeOffset UpdateTime { get; set; }

    /// <summary>
    /// 操作人
    /// </summary>
    public Guid? OperatorUserId { get; set; }

    /// <summary>
    /// 操作人详情
    /// </summary>
    public virtual BMUser? OperatorUser { get; set; }
}

