using MediatR;
using Microsoft.AspNetCore.Mvc;
using wcc.rating.kernel.Models;
using wcc.rating.kernel.RequestHandlers;

namespace wcc.rating.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingController
    {
        protected readonly ILogger<RatingController>? _logger;
        protected readonly IMediator? _mediator;

        public RatingController(ILogger<RatingController>? logger, IMediator? mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public Task<List<RatingModel>> Get()
        {
            return _mediator.Send(new GetRatingQuery());
        }

        [HttpPost]
        public Task Add(List<RatingModel> rating)
        {
            return _mediator.Send(new AddRatingQuery(rating));
        }

        [HttpPost, Route("evolve")]
        public Task Evolve()
        {
            return _mediator.Send(new EvolveRatingQuery());
        }
    }
}
