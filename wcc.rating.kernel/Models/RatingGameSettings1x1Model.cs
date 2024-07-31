using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wcc.rating.Enums;

namespace wcc.rating.kernel.Models
{
    public class RatingGameSettings1x1Model
    {
        public string Nation { get; set; }
        public WCCOptions Option { get; set; }
        public OpponentRating OpponentRating { get; set; }
        public Availability Availability { get; set; }
        public GameType GameType { get; set; }
    }
}
