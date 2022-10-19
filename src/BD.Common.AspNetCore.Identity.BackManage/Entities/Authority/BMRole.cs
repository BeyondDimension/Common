using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD.Common.Entities.Authority
{
    public class BMRole : TenantEntity<Guid>, IOrder
    {
        const int MaxLength_Name = 20;
        const int MaxLength_Describe = 400;

        [MaxLength(MaxLength_Name)]
        [Required]
        public string Name { get; set; }

        [MaxLength(MaxLength_Describe)]
        public string Describe { get; set; }

        public long Order { get; set; }
    }
}
