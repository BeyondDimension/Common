using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD.Common.Entities;

public abstract class CreationEntity<TPrimaryKey> :
    Entity<TPrimaryKey>,
    ICreationTime,
    ICreateUser,
    ICreateUserId
    where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTimeOffset CreationTime { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public Guid CreateUserId { get; set; }

    /// <summary>
    /// 创建人关联
    /// </summary>
    public virtual BMUser? CreateUser { get; set; }

}
