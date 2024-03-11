using Microsoft.AspNetCore.Mvc;

namespace Snapdragon.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExperimentsController : ControllerBase
    {
        private readonly ILogger<ExperimentsController> _logger;
        private readonly ISnapdragonRepository _repository;

        public ExperimentsController(
            ISnapdragonRepository repository,
            ILogger<ExperimentsController> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IReadOnlyList<Data.Experiment>> GetAllAsync()
        {
            var experiments = await _repository.GetExperiments();
            return experiments.Select(e => (Data.Experiment)e).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Data.Experiment>> GetAsync(Guid id)
        {
            var experiment = await _repository.GetExperiment(id);

            if (experiment == null)
            {
                return NotFound();
            }

            return (Data.Experiment)experiment;
        }

        [HttpGet("{id}/populations")]
        public async Task<
            ActionResult<IReadOnlyList<Data.Population>>
        > GetPopulationsForExperimentAsync(Guid id)
        {
            var experiment = await _repository.GetExperiment(id);

            if (experiment == null)
            {
                return NotFound();
            }

            return (await _repository.GetPopulations(id)).Select(Data.Population.From).ToList();
        }

        [HttpGet("{id}/generations/{generation}/games")]
        public async Task<ActionResult<IReadOnlyList<Data.Game>>> GetGamesForGenerationAsync(
            Guid id,
            int generation
        )
        {
            var games = await _repository.GetGames(id, generation);

            return games.Select(g => (Data.Game)g).ToList();
        }
    }
}
