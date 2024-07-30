using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using wcc.rating.kernel.Models;
using wcc.rating.kernel.RequestHandlers;

namespace wcc.rating.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingGameController : ControllerBase
    {
        protected readonly ILogger<RatingGameController>? _logger;
        protected readonly IMediator? _mediator;

        public RatingGameController(ILogger<RatingGameController>? logger, IMediator? mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<List<RatingGame1x1Model>> Get()
        {
            return await _mediator.Send(new GetRatingGamesQuery());
        }

        [HttpGet("{id}")]
        public async Task<RatingGame1x1Model> Get(string id)
        {
            string ratingGameId = HttpUtility.UrlDecode(id);
            return await _mediator.Send(new GetRatingGameQuery(ratingGameId));
        }

        [HttpPost]
        public async Task<bool> Post(RatingGame1x1Model ratingGame)
        {
            return await _mediator.Send(new SaveOrUpdateRatingGameQuery(ratingGame));
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            string ratingGameId = HttpUtility.UrlDecode(id);
            return await _mediator.Send(new DeleteRatingGameQuery(ratingGameId));
        }
    }
}
