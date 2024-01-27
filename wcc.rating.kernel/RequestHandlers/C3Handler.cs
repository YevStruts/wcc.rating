using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using wcc.rating.data;
using wcc.rating.Infrastructure;
using wcc.rating.kernel.Helpers;
using wcc.rating.kernel.Models;
using wcc.rating.kernel.Models.C3;

namespace wcc.rating.kernel.RequestHandlers
{
    public class C3GetRatingQuery : IRequest<List<C3RankModel>>
    {
        public C3GetRatingQuery()
        {
        }
    }

    public class C3SaveGameQuery : IRequest<bool>
    {
        public List<C3GameResultModel> Result { get; }
        public C3SaveGameQuery(List<C3GameResultModel> result)
        {
            this.Result = result;
        }
    }

    public class C3Handler : IRequestHandler<C3GetRatingQuery, List<C3RankModel>>,
        IRequestHandler<C3SaveGameQuery, bool>
    {
        private readonly IDataRepository _db;
        private readonly IMapper _mapper = MapperHelper.Instance;

        public C3Handler(IDataRepository db)
        {
            _db = db;
        }

        public async Task<List<C3RankModel>> Handle(C3GetRatingQuery request, CancellationToken cancellationToken)
        {
            var ranks = _db.GetRanks();
            return _mapper.Map<List<C3RankModel>>(ranks);
        }

        public async Task<bool> Handle(C3SaveGameQuery request, CancellationToken cancellationToken)
        {
            if (request.Result.Any())
            {
                var ranks = _db.GetRanks();

                // if player missing in rank
                var playerIds = request.Result.Select(r => r.player_id).ToArray();
                foreach (var playerId in playerIds)
                {
                    var rank = ranks.FirstOrDefault(r => r.PlayerId == playerId);
                    if (rank == null)
                    {
                        ranks.Add(new Rank
                        {
                            PlayerId = playerId,
                            Score = 1000
                        });
                    }
                }

                // count points
                var winners = request.Result.Where(r => r.result == (int)GameResult.WIN).Select(r => r.player_id).ToList();
                var losers = request.Result.Where(r => r.result == (int)GameResult.LOSE).Select(r => r.player_id).ToList();

                var av_rank_winners = (float)ranks.Where(r => winners.Contains(r.PlayerId)).Sum(r => r.Score) / (float)winners.Count;
                var av_rank_losers = (float)ranks.Where(r => losers.Contains(r.PlayerId)).Sum(r => r.Score) / (float)losers.Count;

                var ordered_ranks = ranks.OrderByDescending(r => r.Score).ToList();

                var kFactor = 32.0;

                var elo_rank = EloHelper.Count(av_rank_winners, av_rank_losers, 1, 32.0, 32.0);

                foreach (var player in winners)
                {
                    var player_rank = ordered_ranks.First(r => r.PlayerId == player);
                    var progress = Convert.ToInt32(elo_rank.Item1 - av_rank_winners);
                    // apply some koef to progress if player is top ranked.
                    ranks.First(p => p.PlayerId == player).Score += progress;
                }

                foreach (var player in losers)
                {
                    var player_rank = ordered_ranks.First(r => r.PlayerId == player);
                    var progress = Convert.ToInt32(elo_rank.Item2 - av_rank_losers);
                    ranks.First(p => p.PlayerId == player).Score += progress;
                }

                _db.SaveRanks(ranks);

                return true;
            }
            return false;
        }
    }
}
