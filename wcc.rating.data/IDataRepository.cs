using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wcc.rating.Infrastructure;

namespace wcc.rating.data
{
    public interface IDataRepository
    {
        bool SaveRating(List<Rating> rating);
        List<Rating> GetRating();

        Game GetGame(long gameId);
        List<Game> GetGames();
        bool SaveGame(Game game);
        bool SaveGames(List<Game> games);
    }
}
