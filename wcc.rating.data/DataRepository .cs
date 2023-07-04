using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wcc.rating.Infrastructure;
using static System.Formats.Asn1.AsnWriter;

namespace wcc.rating.data
{
    public class DataRepository : IDataRepository
    {
        public bool SaveRating(List<Rating> rating)
        {
            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                rating.ForEach(r =>
                {
                    var ratingDto = session.Query<Rating>().Where(x => x.PlayerId == r.PlayerId).FirstOrDefault();
                    if (ratingDto != null)
                    {
                        ratingDto.Points = r.Points;
                    }
                    else
                    {
                        ratingDto = new Rating { PlayerId = r.PlayerId, Points = r.Points };
                        session.Store(ratingDto);
                    }
                });
                session.SaveChanges();
            }
            return true;
        }

        public List<Rating> GetRating()
        {
            List<Rating> rating = new List<Rating>();
            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                rating.AddRange(session.Query<Rating>().ToList());
            }
            return rating;
        }

        public bool SaveGame(Game game)
        {
            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                var gameDto = session.Query<Game>().Where(x => x.GameId == game.GameId).FirstOrDefault();
                if (gameDto != null)
                {
                    gameDto.HPlayerId = game.HPlayerId;
                    gameDto.HScore = game.HScore;
                    gameDto.VPlayerId = game.VPlayerId;
                    gameDto.VScore = game.VScore;
                }
                else
                {
                    gameDto = new Game
                    {
                        GameId = game.GameId,
                        HPlayerId = game.HPlayerId,
                        HScore = game.HScore,
                        VPlayerId = game.VPlayerId,
                        VScore = game.VScore
                    };
                    session.Store(game);
                }
                session.SaveChanges();
            }
            return true;
        }

        public bool SaveGames(List<Game> games)
        {
            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                games.ForEach(game => session.Store(game));

                session.SaveChanges();
            }
            return true;
        }
    }
}
