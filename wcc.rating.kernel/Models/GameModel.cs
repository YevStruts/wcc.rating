using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wcc.rating.Infrastructure;

namespace wcc.rating.kernel.Models
{
    public class GameModel
    {
        public long GameId { get; set; }
        public long HPlayerId { get; set; }
        public int HScore { get; set; }
        public long VPlayerId { get; set; }
        public int VScore { get; set; }
        public GameType GameType { get; set; }
        public List<long>? HParticipants { get; set; }
        public List<long>? VParticipants { get; set; }
    }
}
