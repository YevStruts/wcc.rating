using AutoMapper;
using MediatR;
using wcc.rating.data;
using wcc.rating.Infrastructure;
using wcc.rating.kernel.Helpers;
using wcc.rating.kernel.Models;

namespace wcc.rating.kernel.RequestHandlers
{
    public class SaveGameQuery : IRequest<bool>
    {
        public GameModel Game { get; set; }

        public SaveGameQuery(GameModel game)
        {
            this.Game = game;
        }
    }

    public class GameHandler : IRequestHandler<SaveGameQuery, bool>
    {
        private readonly IDataRepository _db;
        private readonly IMapper _mapper = MapperHelper.Instance;

        public GameHandler(IDataRepository db)
        {
            _db = db;
        }

        public Task<bool> Handle(SaveGameQuery request, CancellationToken cancellationToken)
        {
            Game game = _mapper.Map<Game>(request.Game);

            if (game == null)
                return Task.FromResult(false);

            if (!_db.SaveGame(game))
                return Task.FromResult(false);

            return Task.FromResult(true);

            //var scores = ScoreHelper.GetBO3Score(request.Game.HScore, request.Game.VScore);

            //var rating = _db.GetRating().OrderBy(r => r.Points).ToList();

            //Rating hRating = rating.FirstOrDefault(r => r.PlayerId == request.Game.HPlayerId) ??
            //    new Rating() { PlayerId = request.Game.HPlayerId, Points = 1000 };

            //Rating vRating = rating.FirstOrDefault(r => r.PlayerId == request.Game.VPlayerId) ??
            //    new Rating() { PlayerId = request.Game.VPlayerId, Points = 1000 };

            //var hPosition = rating.IndexOf(hRating);
            //if (hPosition == -1) hPosition = rating.Count + 1;
            //double hFactor = EloHelper.GetKFactor(hPosition);

            //var vPosition = rating.IndexOf(vRating);
            //if (vPosition == -1) vPosition = rating.Count + 1;
            //double vFactor = EloHelper.GetKFactor(vPosition);

            //var newRating = EloHelper.Count(hRating.Points, vRating.Points, scores.Item1, hFactor, vFactor);

            //var hprogress = Convert.ToInt32(newRating.Item1);
            //var vprogress = Convert.ToInt32(newRating.Item2);

            //return Task.FromResult(_db.SaveRating(new List<Rating>
            //{
            //    new Rating { PlayerId = request.Game.HPlayerId, Points = hprogress },
            //    new Rating { PlayerId = request.Game.VPlayerId, Points = vprogress }
            //}));
        }
    }
}
