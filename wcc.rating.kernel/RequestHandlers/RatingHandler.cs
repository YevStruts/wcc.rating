using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Sparrow.Logging;
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
using Microservices = wcc.rating.kernel.Models.Microservices;
using Core = wcc.rating.kernel.Models.Microservices.Core;
using System.Web;
using wcc.rating.kernel.Models.Microservices.Core;

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

    public class EvolveRatingQuery : IRequest<bool>
    {
        public EvolveRatingQuery()
        {
        }
    }

    public class RatingHandler :
        IRequestHandler<GetRatingQuery, List<RatingModel>>,
        IRequestHandler<AddRatingQuery, bool>,
        IRequestHandler<EvolveRatingQuery, bool>
    {
        protected readonly ILogger<RatingHandler>? _logger;
        private readonly IDataRepository _db;
        private readonly IMapper _mapper = MapperHelper.Instance;
        private readonly Microservices.Config _mcsvcConfig;

        public RatingHandler(ILogger<RatingHandler>? logger, IDataRepository db, Microservices.Config mcsvcConfig)
        {
            _logger = logger;
            _db = db;
            _mcsvcConfig = mcsvcConfig;
        }

        public Task<List<RatingModel>> Handle(GetRatingQuery request, CancellationToken cancellationToken)
        {
            List<Rating> rating = _db.GetRating();

            var model = new List<RatingModel>();
            rating.ForEach(r => model.Add(_mapper.Map<RatingModel>(r)));

            #region Calculate rating
            List<Game> games = _db.GetGames().ToList();
            foreach (var game in games)
            {
                if (game.ScoreA == 0 && game.ScoreB == 0)
                    continue;

                if (game.GameType == GameType.Teams &&
                    (game.SideA == null || game.SideB == null ||
                    !game.SideA.Any() || !game.SideB.Any()))
                    continue;

                var scores = ScoreHelper.GetBOScore(game.ScoreA, game.ScoreB);

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

                    /* corection avoid reducing scores if won */
                    if (game.ScoreA > game.ScoreB && hprogress < hRating.Points ||
                        game.ScoreB > game.ScoreA && vprogress < vRating.Points)
                    {
                        hprogress = hRating.Points;
                        vprogress = vRating.Points;
                    }

                    var hPoints = hRating.Points;
                    var vPoints = vRating.Points;

                    model.First(p => p.PlayerId == GetPlayerIdQuickFix(game.SideA.First()).ToString()).Points = hprogress;
                    model.First(p => p.PlayerId == GetPlayerIdQuickFix(game.SideB.First()).ToString()).Points = vprogress;

                    _logger.LogTrace($"-----------------------------");
                    _logger.LogTrace($"Game: {game.SideA.First()} vs {game.SideB.First()} {game.ScoreA}-{game.ScoreB}");
                    _logger.LogTrace($"Player A: {game.SideA.First()} old:{hPoints} new:{hprogress}");
                    _logger.LogTrace($"Player B: {game.SideB.First()} old:{vPoints} new:{vprogress}");
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

        public async Task<bool> Handle(AddRatingQuery request, CancellationToken cancellationToken)
        {
            _db.Clear<Rating>();
            _db.Clear<Game>();

            var rating = _mapper.Map<List<Rating>>(request.Rating);
            return _db.SaveRating(rating) && _db.SaveCheckpoint(new Checkpoint { DateTime = DateTime.UtcNow });
        }

        public async Task<bool> Handle(EvolveRatingQuery request, CancellationToken cancellationToken)
        {
            var players = await new ApiCaller(_mcsvcConfig.CoreUrl).GetAsync<List<Core.PlayerModel>>("api/player");

            var rating = await Handle(new GetRatingQuery(), cancellationToken);

            var date = DateTime.UtcNow;
            foreach (var r in rating)
            {
                var player = players.FirstOrDefault(p => p.Id == r.PlayerId);
                if (player == null) continue;

                var games = await new ApiCaller(_mcsvcConfig.CoreUrl).GetAsync<List<Core.GameModel>>($"api/game/player/{HttpUtility.UrlEncode(player.Id)}");

                var lastGame = games.FirstOrDefault();
                if (lastGame != null)
                {
                    var daysSinceLastGame = (DateTime.UtcNow - lastGame.Scheduled).Days;
                    if (daysSinceLastGame > 30 /* -5 points */)
                    {
                        r.Points -= 5;
                    }
                    if (daysSinceLastGame > 364 /* becoma Innactive */)
                    {
                        player.IsActive = false;
                        await new ApiCaller(_mcsvcConfig.CoreUrl).PostAsync<PlayerModel, bool>($"api/player", player);
                    }
                }
            }
            return await Handle(new AddRatingQuery(rating), cancellationToken);
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
            if ((new string[] { "players/43-A", "players/94-A" }).Contains(playerId) /* fenrir-miracle */) 
                return "players/43-A";
            if ((new string[] { "players/54-A", "players/152-A", "players/173-A" }).Contains(playerId) /* supermati-spoxmati */)
                return "players/173-A"; 
            //if (playerId == 151.ToString() /* DaronirYT */)
            //    return 136.ToString();
            if ((new string[] { "players/154-A", "players/171-A" }).Contains(playerId) /* [PR]ELENDOR */)
                return "players/171-A";
            if ((new string[] { "players/132-A", "players/153-A" }).Contains(playerId) /* Sake */)
                return "players/132-A";
            if ((new string[] { "players/241-A", "players/177-A" }).Contains(playerId) /* Danny */)
                return "players/241-A";
            if ((new string[] { "players/388-A" }).Contains(playerId) /* Husarz */)
                return "players/144-A";
            return playerId;
        }
    }
}
