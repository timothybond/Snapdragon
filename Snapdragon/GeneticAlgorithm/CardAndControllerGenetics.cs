using System.Collections.Immutable;

namespace Snapdragon.GeneticAlgorithm
{
    public record CardAndControllerGenetics(
        ImmutableList<CardDefinition> AllPossibleCards,
        ImmutableList<IPlayerController> AllControllers,
        Func<CardDefinition, int>? OrderBy = null,
        int MutationPer = 100
    ) : Genetics<CardAndControllerGeneSequence>
    {
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
                this.AllPossibleCards.OrderBy(c => Random.Next()).Take(12).ToList(),
                this.AllPossibleCards,
                this.MutationPer,
                this.OrderBy
            );
            var controllerSequence = new ControllerGeneSequence(
                Random.Of(AllControllers),
                AllControllers
            );
            return new CardAndControllerGeneSequence(cardSequence, controllerSequence);
        }
    }
}
