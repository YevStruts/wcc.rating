using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wcc.rating.Infrastructure
{
    public class Rank
    {
        public int RankId { get; set; }
        public long PlayerId { get; set; }
        public int Score { get; set; }
    }
}
