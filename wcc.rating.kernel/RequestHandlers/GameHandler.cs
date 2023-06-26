using AutoMapper;
using MediatR;
using wcc.rating.data;
using wcc.rating.kernel.Helpers;
using wcc.rating.kernel.Models;

namespace wcc.rating.kernel.RequestHandlers
{
    public class GetGameQuery : IRequest<GameModel>
    {
        public long Id { get; }

        public GetGameQuery(long id)
        {
            Id = id;
        }
    }

    public class SaveOrUpdateGameQuery : IRequest<bool>
    {
        public SaveOrUpdateGameQuery()
        {
        }
    }

    public class DeleteGameQuery : IRequest<bool>
    {
        public DeleteGameQuery()
        {
        }
    }

    public class GameHandler :
        IRequestHandler<GetGameQuery, GameModel>,
        IRequestHandler<SaveOrUpdateGameQuery, bool>,
        IRequestHandler<DeleteGameQuery, bool>
    {
        private readonly IDataRepository _db;
        private readonly IMapper _mapper = MapperHelper.Instance;

        public GameHandler(IDataRepository db)
        {
            _db = db;
        }

        public Task<GameModel> Handle(GetGameQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Handle(SaveOrUpdateGameQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Handle(DeleteGameQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
