using wcc.rating.Enums;

namespace wcc.rating.Infrastructure
{
    public class RatingGame1x1 : Document
    {
        public ulong MessageId { get; set; }
        public ulong UserId { get; set; }
        public ulong ChannelId { get; set; }

        public GameStatus Status { get; set; }
        public DateTime Created { get; set; }
        public RatingGameSettings1x1 Settings { get; set; }
    }
}
