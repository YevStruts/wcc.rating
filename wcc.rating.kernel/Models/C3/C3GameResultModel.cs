using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wcc.rating.kernel.Models.C3
{
    public class C3GameResultModel
    {
        public int RankId { get; set; }
        public string? Description { get; set; }
        public List<C3GameItemResultModel>? Items { get; set; }
    }
}
