using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD.Common.Columns
{
    public interface ITenant
    {
        Guid TenantId { get; set; }
    }
}
