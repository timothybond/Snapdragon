namespace Snapdragon.GeneticAlgorithm
{
    public record PartiallyFixedCardGeneSequence(
        FixedCardGeneSequence FixedCards,
        CardGeneSequence EvolvingCards,
        IPlayerController Controller,
        Guid Id,
        Guid? FirstParentId,
        Guid? SecondParentId
    ) : IGeneSequence<PartiallyFixedCardGeneSequence>
    {
        public PartiallyFixedCardGeneSequence Cross(PartiallyFixedCardGeneSequence other)
        {
            return new PartiallyFixedCardGeneSequence(
                FixedCards.Cross(other.FixedCards),
                EvolvingCards.Cross(other.EvolvingCards),
                Controller,
                Guid.NewGuid(),
                this.Id,
                other.Id
            );
        }

        public IReadOnlyList<CardDefinition> GetCards()
        {
            return this.FixedCards.Cards.Concat(this.EvolvingCards.Cards).ToList();
        }

        public string? GetControllerString()
        {
            return Controller.ToString();
        }

        public PlayerConfiguration GetPlayerConfiguration()
        {
            return new PlayerConfiguration(
                Id.ToString(),
                new Deck(FixedCards.Cards.AddRange(EvolvingCards.Cards), Id),
                Controller
            );
        }
    }
}
