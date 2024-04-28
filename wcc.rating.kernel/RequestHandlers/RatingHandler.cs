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
            List<Game> games = _db.GetGames().OrderBy(e => e.GameId).ToList();
            foreach (var game in games)
            {
                if (game.ScoreA == 0 && game.ScoreB == 0)
                    continue;

                if (game.GameType == GameType.Teams &&
                    (game.SideA == null || game.SideB == null ||
                    !game.SideA.Any() || !game.SideB.Any()))
                    continue;

                var scores = ScoreHelper.GetBO3Score(game.ScoreA, game.ScoreB);

                if (game.GameType == GameType.Individual)
                {
                    var hRating = getAndAddRatingIfMissing(game.SideA.First(), ref model);

                    var vRating = getAndAddRatingIfMissing(game.SideB.First(), ref model);

                    var modelOrdered = model.OrderByDescending(p => p.Points).ToList();

                    var hPosition = modelOrdered.IndexOf(hRating);
                    if (hPosition == -1) hPosition = model.Count + 1;
                    double hFactor = EloHelper.GetKFactor(hPosition);

                    var vPosition = modelOrdered.IndexOf(vRating);
                    if (vPosition == -1) vPosition = model.Count + 1;
                    double vFactor = EloHelper.GetKFactor(vPosition);

                    //if (game.IsTechScored)
                    //{
                    //    hFactor /= 2.0;
                    //    vFactor /= 2.0;
                    //}

                    // kFactor of lower rated player
                    double kFactorLower = hPosition > vPosition ? hFactor : vFactor;

                    var newRating = EloHelper.Count(hRating.Points, vRating.Points, scores.Item1,
                        game.ScoreA >= game.ScoreB ? hFactor : kFactorLower,
                        game.ScoreB >= game.ScoreA ? vFactor : kFactorLower);

                    var hprogress = Convert.ToInt32(newRating.Item1);
                    var vprogress = Convert.ToInt32(newRating.Item2);

                    model.First(p => p.PlayerId == GetPlayerIdQuickFix(game.SideA.First()).ToString()).Points = hprogress;
                    model.First(p => p.PlayerId == GetPlayerIdQuickFix(game.SideB.First()).ToString()).Points = vprogress;
                }
                else if (game.GameType == GameType.Teams)
                {
                    List<RatingModel> hRatingTeam = new List<RatingModel>();
                    foreach (var playerId in game.SideA)
                    {
                        hRatingTeam.Add(getAndAddRatingIfMissing(playerId, ref model));
                    }
                    var hRatingAverage = hRatingTeam.Select(r => r.Points).Average();

                    List<RatingModel> vRatingTeam = new List<RatingModel>();
                    foreach (var playerId in game.SideB)
                    {
                        vRatingTeam.Add(getAndAddRatingIfMissing(playerId, ref model));
                    }
                    var vRatingAverage = vRatingTeam.Select(r => r.Points).Average();

                    var modelOrdered = model.OrderByDescending(p => p.Points).ToList();

                    List<double> hFactorList = new List<double>();
                    List<int> hPositionList = new List<int>();
                    foreach (var hRating in hRatingTeam)
                    {
                        var hPosition = modelOrdered.IndexOf(hRating);
                        if (hPosition == -1) hPosition = model.Count + 1;
                        hPositionList.Add(hPosition);
                        hFactorList.Add(EloHelper.GetKFactor(hPosition));
                    }
                    var hFactor = hFactorList.Average();

                    List<double> vFactorList = new List<double>();
                    List<int> vPositionList = new List<int>();
                    foreach (var vRating in vRatingTeam)
                    {
                        var vPosition = modelOrdered.IndexOf(vRating);
                        if (vPosition == -1) vPosition = model.Count + 1;
                        vPositionList.Add(vPosition);
                        vFactorList.Add(EloHelper.GetKFactor(vPosition));
                    }
                    var vFactor = vFactorList.Average();

                    //if (game.IsTechScored)
                    //{
                    //    hFactor /= 2.0;
                    //    vFactor /= 2.0;
                    //}

                    // kFactor of lower rated player
                    double kFactorLower =
                        hPositionList.Average() > vPositionList.Average() ?
                        hFactor : vFactor;

                    // Rating progress
                    var newRating = EloHelper.Count(hRatingAverage, vRatingAverage, scores.Item1,
                        game.ScoreA >= game.ScoreB ? hFactor : kFactorLower,
                        game.ScoreB >= game.ScoreA ? vFactor : kFactorLower);

                    var hprogress = Convert.ToInt32(newRating.Item1 - hRatingAverage);
                    var vprogress = Convert.ToInt32(newRating.Item2 - vRatingAverage);

                    // if team won 1-0 and has regress then set progress equeals 0
                    if (game.ScoreA >= game.ScoreB && hprogress < 0) hprogress /= 5;
                    if (game.ScoreA <= game.ScoreB && hprogress > 0) hprogress /= 5;

                    if (game.ScoreB >= game.ScoreA && vprogress < 0) vprogress /= 5;
                    if (game.ScoreB <= game.ScoreA && vprogress > 0) vprogress /= 5;

                    // if lose reduce lost points twice
                    if (game.ScoreA >= game.ScoreB) vprogress /= 2;
                    if (game.ScoreB >= game.ScoreA) hprogress /= 2;

                    foreach (var playerId in game.SideA)
                    {
                        model.First(p => p.PlayerId == GetPlayerIdQuickFix(playerId).ToString()).Points += hprogress;
                    }

                    foreach (var playerId in game.SideB)
                    {
                        model.First(p => p.PlayerId == GetPlayerIdQuickFix(playerId).ToString()).Points += vprogress;
                    }
                }
            }
            #endregion
            
            return Task.FromResult(model.OrderBy(r => r.Points).ToList());
        }

        public Task<bool> Handle(AddRatingQuery request, CancellationToken cancellationToken)
        {
            _db.Clear<Rating>();
            _db.Clear<Game>();

            var rating = _mapper.Map<List<Rating>>(request.Rating);
            return Task.FromResult(_db.SaveRating(rating));
        }

        private RatingModel getAndAddRatingIfMissing(string playerId, ref List<RatingModel> model)
        {
            if (!model.Any(r => r.PlayerId == GetPlayerIdQuickFix(playerId).ToString()))
            {
                model.Add(new RatingModel() { PlayerId = playerId.ToString(), Points = 1000 });
            }

            return model.First(r => r.PlayerId == GetPlayerIdQuickFix(playerId).ToString());
        }

        private string GetPlayerIdQuickFix(string playerId)
        {
            if (playerId == 97.ToString() /* fenrir-miracle */) 
                return 44.ToString();
            if (playerId == 157.ToString() /* supermati-spoxmati */)
                return 56.ToString(); 
            if (playerId == 151.ToString() /* DaronirYT */)
                return 136.ToString();
            if (playerId == 183.ToString() /* [PR]ELENDOR */)
                return 159.ToString();
            if (playerId == 158.ToString() /* Sake */)
                return 135.ToString();
            return playerId;
        }
    }
}
