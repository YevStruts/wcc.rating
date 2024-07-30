using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using wcc.rating.data;
using wcc.rating.Infrastructure;
using wcc.rating.kernel.Helpers;
using wcc.rating.kernel.Models;

namespace wcc.rating.kernel.RequestHandlers
{
    public class GetRatingGamesQuery : IRequest<List<RatingGame1x1Model>>
    {
        public GetRatingGamesQuery()
        {
        }
    }

    public class GetRatingGameQuery : IRequest<RatingGame1x1Model>
    {
        public string Id { get; private set; }
        public GetRatingGameQuery(string id)
        {
            Id = id;
        }
    }

    public class SaveOrUpdateRatingGameQuery : IRequest<bool>
    {
        public RatingGame1x1Model Game { get; private set; }
        public SaveOrUpdateRatingGameQuery(RatingGame1x1Model game)
        {
            Game = game;
        }
    }

    public class AddRatingGameQuery : IRequest<bool>
    {
        public RatingGame1x1Model Game { get; private set; }
        public AddRatingGameQuery(RatingGame1x1Model game)
        {
            Game = game;
        }
    }

    public class DeleteRatingGameQuery : IRequest<bool>
    {
        public string Id { get; private set; }
        public DeleteRatingGameQuery(string id)
        {
            Id = id;
        }
    }

    public class RatingGameHandler :
        IRequestHandler<GetRatingGamesQuery, List<RatingGame1x1Model>>,
        IRequestHandler<GetRatingGameQuery, RatingGame1x1Model>,
        IRequestHandler<SaveOrUpdateRatingGameQuery, bool>,
        IRequestHandler<DeleteRatingGameQuery, bool>
    {
        protected readonly ILogger<RatingGameHandler>? _logger;
        private readonly IDataRepository _db;
        private readonly IMapper _mapper = MapperHelper.Instance;

        public RatingGameHandler(ILogger<RatingGameHandler>? logger, IDataRepository db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<List<RatingGame1x1Model>> Handle(GetRatingGamesQuery request, CancellationToken cancellationToken)
        {
            List<RatingGame1x1> ratingGameDtos = _db.GetRatingGames();
            var model = new List<RatingGame1x1Model>();
            ratingGameDtos.ForEach(r => model.Add(_mapper.Map<RatingGame1x1Model>(r)));
            return model;
        }

        public async Task<RatingGame1x1Model> Handle(GetRatingGameQuery request, CancellationToken cancellationToken)
        {
            RatingGame1x1? ratingGameDto = _db.GetRatingGame(request.Id);
            if (ratingGameDto != null)
                return _mapper.Map<RatingGame1x1Model>(ratingGameDto);
            return null;
        }

        public async Task<bool> Handle(SaveOrUpdateRatingGameQuery request, CancellationToken cancellationToken)
        {
            RatingGame1x1 ratingGameDto = _mapper.Map<RatingGame1x1>(request.Game);
            return _db.SaveRatingGame(ratingGameDto);
        }

        public async Task<bool> Handle(DeleteRatingGameQuery request, CancellationToken cancellationToken)
        {
            return _db.DeleteRatingGame(request.Id);
        }
    }
}
