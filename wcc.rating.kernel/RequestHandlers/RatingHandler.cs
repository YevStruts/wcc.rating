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
            var model = new List<RatingModel>()
            {
                new RatingModel() { PlayerId = 1, Points = 1000 }
            };
            return Task.FromResult(model);
        }

        public Task<bool> Handle(AddRatingQuery request, CancellationToken cancellationToken)
        {
            var rating = _mapper.Map<List<Rating>>(request.Rating);
            return Task.FromResult(_db.SaveRating(rating));
        }
    }
}
