using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wcc.rating.Infrastructure;
using static System.Collections.Specialized.BitVector32;
using static System.Formats.Asn1.AsnWriter;

namespace wcc.rating.data
{
    public class DataRepository : IDataRepository
    {
        public bool SaveRating(List<Rating> rating)
        {
            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                var prevRatings = session.Query<Rating>().ToList();
                rating.ForEach(r =>
                {
                    var ratingDto = prevRatings.Where(x => x.PlayerId == r.PlayerId).FirstOrDefault();
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

        public void Clear<T>()
        {
            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                var objects = session.Query<T>().ToList();
                while (objects.Any())
                {
                    foreach (var obj in objects)
                    {
                        session.Delete(obj);
                    }

                    session.SaveChanges();
                    objects = session.Query<T>().ToList();
                }
            }
        }

        public Game? GetGame(string gameId)
        {
            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                return session.Query<Game>().FirstOrDefault(g => g.GameId == gameId);
            }
        }

        public List<Game> GetGames()
        {
            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                return session.Query<Game>().ToList();
            }
        }

        public bool SaveGame(Game game)
        {
            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                var gameDto = session.Query<Game>().Where(x => x.GameId == game.GameId).FirstOrDefault();
                if (gameDto != null)
                {
                    gameDto.SideA = game.SideA;
                    gameDto.ScoreA = game.ScoreA;
                    gameDto.SideB = game.SideB;
                    gameDto.ScoreB = game.ScoreB;
                }
                else
                {
                    gameDto = new Game
                    {
                        GameId = game.GameId,
                        SideA = game.SideA,
                        ScoreA = game.ScoreA,
                        SideB = game.SideB,
                        ScoreB = game.ScoreB
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

        #region Rank

        public Rank? GetRank(string playerId)
        {
            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                return session.Query<Rank>().FirstOrDefault(/* g => g.GameId == seasonId */);
            }
        }

        public List<Rank> GetRanks()
        {
            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                return session.Query<Rank>().ToList();
            }
        }

        public bool SaveRanks(int rankId, List<Rank> ranks)
        {
            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                var prevRanks = session.Query<Rank>().Where(r => r.RankId == rankId).ToList();
                ranks.ForEach(r =>
                {
                    var rankDto = prevRanks.Where(x => x.PlayerId == r.PlayerId).FirstOrDefault();
                    if (rankDto != null)
                    {
                        rankDto.Score = r.Score;
                    }
                    else
                    {
                        rankDto = new Rank { PlayerId = r.PlayerId, Score = r.Score };
                        session.Store(rankDto);
                    }
                });
                session.SaveChanges();
            }
            return true;
        }

        #endregion
    }
}
