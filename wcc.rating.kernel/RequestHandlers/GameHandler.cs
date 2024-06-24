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

    public class DeleteGameQuery : IRequest<bool>
    {
        public string GameId { get; set; }

        public DeleteGameQuery(string gameId)
        {
            this.GameId = gameId;
        }
    }

    public class DeleteGameByCoreGameIdQuery : IRequest<bool>
    {
        public string GameId { get; set; }

        public DeleteGameByCoreGameIdQuery(string gameId)
        {
            this.GameId = gameId;
        }
    }

    public class GameHandler : IRequestHandler<SaveGameQuery, bool>,
        IRequestHandler<DeleteGameQuery, bool>,
        IRequestHandler<DeleteGameByCoreGameIdQuery, bool>
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
        }

        public async Task<bool> Handle(DeleteGameQuery request, CancellationToken cancellationToken)
        {
            return _db.DeleteGame(request.GameId);
        }

        public async Task<bool> Handle(DeleteGameByCoreGameIdQuery request, CancellationToken cancellationToken)
        {
            return _db.DeleteGameByCoreGameId(request.GameId);
        }
    }
}
