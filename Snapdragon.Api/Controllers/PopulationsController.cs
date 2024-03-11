using Microsoft.AspNetCore.Mvc;
using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PopulationsController : ControllerBase
    {
        private readonly ILogger<ExperimentsController> _logger;
        private readonly ISnapdragonRepository _repository;

        public PopulationsController(
            ISnapdragonRepository repository,
            ILogger<ExperimentsController> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Data.Population>> GetAsync(Guid id)
        {
            var pop = await _repository.GetPopulation<PartiallyFixedCardGeneSequence>(id);

            if (pop == null)
            {
                return NotFound();
            }

            return Data.Population.From(pop);
        }

        [HttpGet("{id}/generations/{generation}")]
        public async Task<ActionResult<IReadOnlyList<Data.Item>>> GetItemsForGenerationAsync(
            Guid id,
            int generation
        )
        {
            var items = await _repository.GetItems<PartiallyFixedCardGeneSequence>(id, generation);

            return items.Select(i => Data.Item.From(i)).ToList();
        }

        [HttpGet("{id}/statistics")]
        public async Task<ActionResult<IReadOnlyList<Data.CardCount>>> GetStatisticsAsync(Guid id)
        {
            var pop = await _repository.GetPopulation<PartiallyFixedCardGeneSequence>(id);

            if (pop == null)
            {
                return NotFound();
            }

            var cardCounts = await _repository.GetCardCounts<PartiallyFixedCardGeneSequence>(id);

            if (cardCounts == null)
            {
                return NotFound();
            }

            return cardCounts
                .Select(cc => new Data.CardCount { Name = cc.Name, Counts = cc.Counts })
                .ToList();
        }
    }
}
