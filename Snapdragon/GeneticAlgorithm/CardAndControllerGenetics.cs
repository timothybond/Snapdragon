using System.Collections.Immutable;

namespace Snapdragon.GeneticAlgorithm
{
    public class CardAndControllerGenetics : Genetics<CardAndControllerGeneSequence>
    {
        private readonly List<IPlayerController> allControllers;
        private readonly int mutationPer;
        private readonly Func<CardDefinition, int>? orderBy;

        private readonly IReadOnlyList<CardDefinition> allPossibleCards;

        public CardAndControllerGenetics(
            int mutationPer = 100,
            Func<CardDefinition, int>? orderBy = null,
            int monteCarloSimulationCount = 5
        )
        {
            this.allControllers = new List<IPlayerController>
            {
                new RandomPlayerController(),
                new MonteCarloSearchController(monteCarloSimulationCount)
            };
            this.allPossibleCards = GetInitialCardDefinitions();
            this.mutationPer = mutationPer;
            this.orderBy = orderBy;
        }

        public override PlayerConfiguration GetPlayerConfiguration(
            CardAndControllerGeneSequence item,
            int index
        )
        {
            return new PlayerConfiguration(
                $"Deck {index}",
                new Deck(item.Cards.Cards.ToImmutableList()),
                item.Controller.Controller
            );
        }

        public override CardAndControllerGeneSequence GetRandomItem()
        {
            var cardSequence = new CardGeneSequence(
                this.allPossibleCards.OrderBy(c => Random.Next()).Take(12).ToList(),
                this.allPossibleCards,
                this.mutationPer,
                this.orderBy
            );
            var controllerSequence = new ControllerGeneSequence(
                Random.Of(allControllers),
                allControllers
            );
            return new CardAndControllerGeneSequence(cardSequence, controllerSequence);
        }
    }
}
