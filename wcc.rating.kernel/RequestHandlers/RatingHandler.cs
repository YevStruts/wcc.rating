using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wcc.rating.data;
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

    public class RatingHandler : IRequestHandler<GetRatingQuery, List<RatingModel>>
    {
        private readonly IDataRepository _db;
        private readonly IMapper _mapper = MapperHelper.Instance;

        public RatingHandler(IDataRepository db)
        {
            _db = db;
        }

        public Task<List<RatingModel>> Handle(GetRatingQuery request, CancellationToken cancellationToken)
        {
            var model = new List<RatingModel>()
            {
                new RatingModel() { Id = 1, Name = "Player 1", Points = 1000 }
            };
            return Task.FromResult(model);
        }
    }
}
