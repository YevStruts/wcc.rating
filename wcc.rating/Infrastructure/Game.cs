using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wcc.rating.Infrastructure
{
    public class Game
    {
        public long GameId { get; set; }
        public long HPlayerId { get; set; }
        public int HScore { get; set; }
        public long VPlayerId { get; set; }
        public int VScore { get; set; }
        public bool IsTechScored { get; set; }
    }
}
