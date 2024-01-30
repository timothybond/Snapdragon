using System.Collections.Immutable;

namespace Snapdragon.GeneticAlgorithm
{
    public class CardGenetics : Genetics<CardGeneSequence>
    {
        private readonly int mutationPer;
        private readonly Func<CardDefinition, int>? orderBy;

        private readonly IReadOnlyList<CardDefinition> allPossibleCards;

        public CardGenetics(int mutationPer = 100, Func<CardDefinition, int>? orderBy = null)
        {
            this.allPossibleCards = GetInitialCardDefinitions();
            this.mutationPer = mutationPer;
            this.orderBy = orderBy;
        }

        public override PlayerConfiguration GetPlayerConfiguration(CardGeneSequence item, int index)
        {
            return new PlayerConfiguration(
                $"Deck {index}",
                new Deck(item.Cards.ToImmutableList()),
                new RandomPlayerController()
            );
        }

        public override CardGeneSequence GetRandomItem()
        {
            return new CardGeneSequence(
                this.allPossibleCards.OrderBy(c => Random.Next()).Take(12).ToList(),
                this.allPossibleCards,
                this.mutationPer,
                this.orderBy
            );
        }
    }
}
