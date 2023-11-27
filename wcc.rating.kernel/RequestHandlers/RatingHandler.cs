using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using wcc.rating.data;
using wcc.rating.Infrastructure;
using wcc.rating.kernel.Helpers;
using wcc.rating.kernel.Models;

namespace wcc.rating.kernel.RequestHandlers
{
    public class GetRatingQuery : IRequest<List<RatingModel>>
    {
        public GetRatingQuery()
        {
        }
    }

    public class AddRatingQuery : IRequest<bool>
    {
        public List<RatingModel> Rating { get; private set; }

        public AddRatingQuery(List<RatingModel> rating)
        {
            this.Rating = rating;
        }
    }

    public class RatingHandler :
        IRequestHandler<GetRatingQuery, List<RatingModel>>,
        IRequestHandler<AddRatingQuery, bool>
    {
        private readonly IDataRepository _db;
        private readonly IMapper _mapper = MapperHelper.Instance;

        public RatingHandler(IDataRepository db)
        {
            _db = db;
        }

        public Task<List<RatingModel>> Handle(GetRatingQuery request, CancellationToken cancellationToken)
        {
            List<Rating> rating = _db.GetRating();

            var model = new List<RatingModel>();
            rating.ForEach(r => model.Add(_mapper.Map<RatingModel>(r)));

            #region Calculate rating
            List<Game> games = _db.GetGames();
            foreach (var game in games)
            {
                if (game.HScore == 0 && game.VScore == 0)
                    continue;

                if (game.GameType == GameType.Individual)
                {
                    var scores = ScoreHelper.GetBO3Score(game.HScore, game.VScore);

                    var hRating = getAndAddRatingIfMissing(game.HPlayerId, ref model);

                    var vRating = getAndAddRatingIfMissing(game.VPlayerId, ref model);

                    var modelOrdered = model.OrderByDescending(p => p.Points).ToList();

                    var hPosition = modelOrdered.IndexOf(hRating);
                    if (hPosition == -1) hPosition = model.Count + 1;
                    double hFactor = EloHelper.GetKFactor(hPosition);

                    var vPosition = modelOrdered.IndexOf(vRating);
                    if (vPosition == -1) vPosition = model.Count + 1;
                    double vFactor = EloHelper.GetKFactor(vPosition);

                    if (game.IsTechScored)
                    {
                        hFactor /= 2.0;
                        vFactor /= 2.0;
                    }

                    // kFactor of lower rated player
                    double kFactorLower = hPosition > vPosition ? hFactor : vFactor;

                    var newRating = EloHelper.Count(hRating.Points, vRating.Points, scores.Item1,
                        game.HScore >= game.VScore ? hFactor : kFactorLower,
                        game.VScore >= game.HScore ? vFactor : kFactorLower);

                    var hprogress = Convert.ToInt32(newRating.Item1);
                    var vprogress = Convert.ToInt32(newRating.Item2);

                    model.First(p => p.PlayerId == GetPlayerIdQuickFix(game.HPlayerId)).Points = hprogress;
                    model.First(p => p.PlayerId == GetPlayerIdQuickFix(game.VPlayerId)).Points = vprogress;
                }
            }
            #endregion
            
            return Task.FromResult(model.OrderBy(r => r.Points).ToList());
        }

        public Task<bool> Handle(AddRatingQuery request, CancellationToken cancellationToken)
        {
            var rating = _mapper.Map<List<Rating>>(request.Rating);
            return Task.FromResult(_db.SaveRating(rating));
        }

        private RatingModel getAndAddRatingIfMissing(long playerId, ref List<RatingModel> model)
        {
            if (!model.Any(r => r.PlayerId == GetPlayerIdQuickFix(playerId)))
            {
                model.Add(new RatingModel() { PlayerId = playerId, Points = 1000 });
            }

            return model.First(r => r.PlayerId == GetPlayerIdQuickFix(playerId));
        }

        private long GetPlayerIdQuickFix(long playerId)
        {
            if (playerId == 97 /* fenrir-miracle */) 
                return 44;
            if (playerId == 157 /* supermati-spoxmati */)
                return 56;
            return playerId;
        }
    }
}
