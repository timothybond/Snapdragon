using System.Collections.Immutable;

namespace Snapdragon.GeneticAlgorithm
{
    public record CardAndControllerGenetics(
        ImmutableList<CardDefinition> AllPossibleCards,
        ImmutableList<IPlayerController> AllControllers,
        Func<CardDefinition, int>? OrderBy = null,
        int MutationPer = 100
    ) : Genetics<CardAndControllerGeneSequence>(AllPossibleCards)
    {
        public override CardAndControllerGeneSequence GetRandomItem()
        {
            var cardSequence = new CardGeneSequence(
                this.AllPossibleCards.OrderBy(c => Random.Next()).Take(12).ToList(),
                this.AllPossibleCards,
                Guid.NewGuid(),
                this.MutationPer,
                this.OrderBy
            );
            var controllerSequence = new ControllerGeneSequence(
                Random.Of(AllControllers),
                AllControllers
            );
            return new CardAndControllerGeneSequence(
                cardSequence,
                controllerSequence,
                Guid.NewGuid()
            );
        }

        protected override IReadOnlyList<CardDefinition> GetCards(
            CardAndControllerGeneSequence item
        )
        {
            return item.Cards.Cards;
        }
    }
}
