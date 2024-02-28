using Microsoft.AspNetCore.Mvc;

namespace Snapdragon.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CardsController : ControllerBase
    {
        private readonly ILogger<ExperimentsController> _logger;
        private readonly ISnapdragonRepository _repository;

        public CardsController(
            ISnapdragonRepository repository,
            ILogger<ExperimentsController> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<Data.Game>> GetAsync(Guid id)
        {
            var gameRecord = await _repository.GetGame(id);
            if (gameRecord == null)
            {
                return NotFound();
            }

            var game = (Data.Game)gameRecord;

            var gameLogs = await _repository.GetGameLogs(id);
            game.Logs = gameLogs.OrderBy(gl => gl.Order).Select(gl => (Data.GameLog)gl).ToList();

            return game;
        }
    }
}
