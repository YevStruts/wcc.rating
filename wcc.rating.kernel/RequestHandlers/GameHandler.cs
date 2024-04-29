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
        }
    }
}
