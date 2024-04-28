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
        public string? GameId { get; set; }
        public GameType GameType { get; set; }
        public List<string>? SideA { get; set; }
        public List<string>? SideB { get; set; }
        public int ScoreA { get; set; }
        public int ScoreB { get; set; }
    }
}
