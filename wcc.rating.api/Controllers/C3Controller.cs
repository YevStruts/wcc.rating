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

        [HttpGet, Route("Rating/{id}")]
        public async Task<C3RankModel> Get(int id)
        {
            return await _mediator.Send(new C3GetRatingQuery(id));
        }

        [HttpPost, Route("Save")]
        public async Task<List<C3SaveRankModel>> Save(C3GameResultModel model)
        {
            return await _mediator.Send(new C3SaveGameQuery(model.RankId, model.Items));
        }
    }
}
