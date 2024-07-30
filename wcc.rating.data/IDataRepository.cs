using wcc.rating.Infrastructure;

namespace wcc.rating.data
{
    public interface IDataRepository
    {
        bool SaveRating(List<Rating> rating);
        List<Rating> GetRating();
        Game GetGame(string gameId);
        List<Game> GetGames();
        bool SaveGame(Game game);
        bool SaveGames(List<Game> games);
        bool DeleteGameByCoreGameId(string gameId);
        bool DeleteGame(string gameId);

        Rank GetRank(string playerId);
        List<Rank> GetRanks();
        bool SaveRanks(int rankId, List<Rank> ranks);
        void Clear<T>();

        bool SaveCheckpoint(Checkpoint game);

        /* RatingGame */
        List<RatingGame1x1> GetRatingGames();
        RatingGame1x1? GetRatingGame(string id);
        bool SaveRatingGame(RatingGame1x1 ratingGame);
        bool DeleteRatingGame(string id);
    }
}
