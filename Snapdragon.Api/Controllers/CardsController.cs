﻿using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<IReadOnlyList<Data.Card>>> GetAsync()
        {
            return (await _repository.GetCardDefinitions()).Select(cd => (Data.Card)cd).ToList();
        }
    }
}
