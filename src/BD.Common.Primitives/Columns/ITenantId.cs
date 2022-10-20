using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD.Common.Columns
{
    public interface ITenantId
    {
        Guid TenantId { get; set; }
    }
}
