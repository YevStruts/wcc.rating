using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wcc.rating.kernel.Models.C3
{
    public class C3SaveRankModel
    {
        public long PlayerId { get; set; }
        public int OldScore { get; set; }
        public int NewScore { get; set; }
    }
}
