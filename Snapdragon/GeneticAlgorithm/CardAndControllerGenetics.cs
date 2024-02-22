using System.Collections.Immutable;

namespace Snapdragon.GeneticAlgorithm
{
    public record CardAndControllerGenetics(
        ImmutableList<CardDefinition> AllPossibleCards,
        ImmutableList<IPlayerController> AllControllers,
        ICardOrder OrderBy,
        int MutationPer = 100
    ) : Genetics<CardAndControllerGeneSequence>(AllPossibleCards, MutationPer, OrderBy)
    {
        public override string GetControllerString()
        {
            throw new InvalidOperationException(
                "Cannot get a single controller string for a genetics instance with variable controllers."
            );
        }

        public override ImmutableList<CardDefinition> GetFixedCards()
        {
            return ImmutableList<CardDefinition>.Empty;
        }

        public override CardAndControllerGeneSequence GetRandomItem()
        {
            var cardSequence = new CardGeneSequence(
                this.AllPossibleCards.OrderBy(c => Random.Next()).Take(12).ToList(),
                this.AllPossibleCards,
                Guid.NewGuid(),
                this.MutationPer,
                this.OrderBy,
                null,
                null
            );
            var controllerSequence = new ControllerGeneSequence(
                Random.Of(AllControllers),
                AllControllers,
                Guid.Empty, // These aren't used in a way that requires tracking
                null,
                null
            );
            return new CardAndControllerGeneSequence(
                cardSequence,
                controllerSequence,
                Guid.NewGuid(),
                null,
                null
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
