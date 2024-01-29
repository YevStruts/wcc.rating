using MediatR;
using Microsoft.AspNetCore.Mvc;
using wcc.rating.kernel.Models;
using wcc.rating.kernel.Models.C3;
using wcc.rating.kernel.RequestHandlers;

namespace wcc.rating.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class C3Controller : Controller
    {
        protected readonly ILogger<C3Controller>? _logger;
        protected readonly IMediator? _mediator;

        public C3Controller(ILogger<C3Controller>? logger, IMediator? mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet, Route("Rating")]
        public async Task<List<C3RankModel>> Get()
        {
            return await _mediator.Send(new C3GetRatingQuery());
        }

        [HttpPost, Route("Save")]
        public async Task<List<C3SaveRankModel>> Save(List<C3GameResultModel> model)
        {
            return await _mediator.Send(new C3SaveGameQuery(model));
        }
    }
}
