using MediatR;
using Microsoft.AspNetCore.Mvc;
using wcc.rating.kernel.Models;
using wcc.rating.kernel.RequestHandlers;

namespace wcc.rating.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController
    {
        protected readonly ILogger<GameController>? _logger;
        protected readonly IMediator? _mediator;

        public GameController(ILogger<GameController>? logger, IMediator? mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost, Route("Save")]
        public Task Save(GameModel model)
        {
            return _mediator.Send(new SaveGameQuery(model));
        }
    }
}
