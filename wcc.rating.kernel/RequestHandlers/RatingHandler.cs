using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
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
                var scores = ScoreHelper.GetBO3Score(game.HScore, game.VScore);

                var hRating = model.FirstOrDefault(r => r.PlayerId == game.HPlayerId) ??
                    new RatingModel() { PlayerId = game.HPlayerId, Points = 1000 };

                var vRating = model.FirstOrDefault(r => r.PlayerId == game.VPlayerId) ??
                    new RatingModel() { PlayerId = game.VPlayerId, Points = 1000 };

                var hPosition = model.IndexOf(hRating);
                if (hPosition == -1) hPosition = model.Count + 1;
                double hFactor = EloHelper.GetKFactor(hPosition);

                var vPosition = model.IndexOf(vRating);
                if (vPosition == -1) vPosition = model.Count + 1;
                double vFactor = EloHelper.GetKFactor(vPosition);

                var newRating = EloHelper.Count(hRating.Points, vRating.Points, scores.Item1, hFactor, vFactor);

                var hprogress = Convert.ToInt32(newRating.Item1);
                var vprogress = Convert.ToInt32(newRating.Item2);

                model.First(p => p.PlayerId == game.HPlayerId).Points = hprogress;
                model.First(p => p.PlayerId == game.VPlayerId).Points = vprogress;
            }
            #endregion
            
            return Task.FromResult(model.OrderBy(r => r.Points).ToList());
        }

        public Task<bool> Handle(AddRatingQuery request, CancellationToken cancellationToken)
        {
            var rating = _mapper.Map<List<Rating>>(request.Rating);
            return Task.FromResult(_db.SaveRating(rating));
        }
    }
}
