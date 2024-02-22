using System.Collections.Immutable;

namespace Snapdragon.GeneticAlgorithm
{
    public record CardGenetics(
        ImmutableList<CardDefinition> AllPossibleCards,
        IPlayerController Controller,
        int MutationPer,
        ICardOrder OrderBy,
        int Length = 12
    ) : Genetics<CardGeneSequence>(AllPossibleCards, MutationPer, OrderBy)
    {
        public override string? GetControllerString()
        {
            return Controller.ToString();
        }

        public override ImmutableList<CardDefinition> GetFixedCards()
        {
            return ImmutableList<CardDefinition>.Empty;
        }

        public PlayerConfiguration GetPlayerConfiguration(CardGeneSequence item, int index)
        {
            return new PlayerConfiguration(
                $"Deck {index}",
                new Deck(item.Cards.ToImmutableList(), item.Id),
                Controller
            );
        }

        public override CardGeneSequence GetRandomItem()
        {
            return new CardGeneSequence(
                this.AllPossibleCards.OrderBy(c => Random.Next()).Take(Length).ToList(),
                this.AllPossibleCards,
                Guid.NewGuid(),
                this.MutationPer,
                this.OrderBy,
                null,
                null,
                this.Controller
            );
        }

        protected override IReadOnlyList<CardDefinition> GetCards(CardGeneSequence item)
        {
            return item.Cards;
        }
    }
}
