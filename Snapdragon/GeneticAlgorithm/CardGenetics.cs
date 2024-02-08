using System.Collections.Immutable;

namespace Snapdragon.GeneticAlgorithm
{
    public record CardGenetics(
        ImmutableList<CardDefinition> AllPossibleCards,
        IPlayerController Controller,
        int MutationPer = 100,
        Func<CardDefinition, int>? OrderBy = null,
        int Length = 12
    ) : Genetics<CardGeneSequence>(AllPossibleCards)
    {
        public PlayerConfiguration GetPlayerConfiguration(CardGeneSequence item, int index)
        {
            return new PlayerConfiguration(
                $"Deck {index}",
                new Deck(item.Cards.ToImmutableList()),
                Controller
            );
        }

        public override CardGeneSequence GetRandomItem()
        {
            return new CardGeneSequence(
                this.AllPossibleCards.OrderBy(c => Random.Next()).Take(Length).ToList(),
                this.AllPossibleCards,
                this.MutationPer,
                this.OrderBy,
                this.Controller
            );
        }

        protected override IReadOnlyList<CardDefinition> GetCards(CardGeneSequence item)
        {
            return item.Cards;
        }
    }
}
