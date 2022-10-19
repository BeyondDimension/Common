using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD.Common.Entities;

public class MenuButton : Entity<Guid>, IOrder
{

    /// <summary>
    /// 租户
    /// </summary>
    public Guid TenantId { get; set; }

    public Guid MenuId { get; set; }

    public Guid BottonId { get; set; }

    public long Order { get; set; }
}
