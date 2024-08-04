using wcc.rating.Enums;

namespace wcc.rating.Infrastructure
{
    public class RatingGameSettings1x1
    {
        public string Nation { get; set; }
        public WCCOptions Option { get; set; }
        public OpponentRating OpponentRating { get; set; }
        public Availability Availability { get; set; }
        public WinRule WinRule { get; set; }
    }
}
