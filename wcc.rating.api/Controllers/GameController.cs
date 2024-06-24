using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Web;
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

        [HttpPost, Route("save")]
        public async Task<bool> Save(GameModel model)
        {
            return await _mediator.Send(new SaveGameQuery(model));
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            string gameId = HttpUtility.UrlDecode(id);
            return await _mediator.Send(new DeleteGameQuery(gameId));
        }

        [HttpPost("ByCoreGameId/{id}")]
        public async Task<bool> DeleteByCoreGameId(string id)
        {
            string gameId = HttpUtility.UrlDecode(id);
            return await _mediator.Send(new DeleteGameByCoreGameIdQuery(gameId));
        }
    }
}
