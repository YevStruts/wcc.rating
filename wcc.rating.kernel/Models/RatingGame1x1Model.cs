using wcc.rating.Enums;
using wcc.rating.Infrastructure;

namespace wcc.rating.kernel.Models
{
    public class RatingGame1x1Model
    {
        public string Id { get; set; }
        public ulong MessageId { get; set; }
        public ulong UserId { get; set; }
        public ulong ChannelId { get; set; }

        public GameStatus Status { get; set; }
        public DateTime Created { get; set; }
        public int Rating { get; set; }
        public RatingGameSettings1x1Model Settings { get; set; }
    }
}
