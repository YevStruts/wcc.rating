

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace wcc.rating.Infrastructure
{
    public class Game
    {
        public Game()
        {
            GameType = GameType.Individual;
        }
        public string GameId { get; set; }
        public GameType GameType { get; set; }
        public List<string> SideA { get; set; }
        public List<string> SideB { get; set; }
        public int ScoreA { get; set; }
        public int ScoreB { get; set; }
    }
}
