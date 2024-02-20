using System.Collections.Immutable;

namespace Snapdragon.GeneticAlgorithm
{
    public record CardAndControllerGeneSequence(
        CardGeneSequence Cards,
        ControllerGeneSequence Controller,
        Guid Id
    ) : IGeneSequence<CardAndControllerGeneSequence>
    {
        public CardAndControllerGeneSequence Cross(CardAndControllerGeneSequence other)
        {
            return new CardAndControllerGeneSequence(
                this.Cards.Cross(other.Cards),
                this.Controller.Cross(other.Controller),
                Guid.NewGuid()
            );
        }

        public IReadOnlyList<CardDefinition> GetCards()
        {
            return this.Cards.Cards;
        }

        public PlayerConfiguration GetPlayerConfiguration()
        {
            return new PlayerConfiguration(
                Id.ToString(),
                new Deck(Cards.Cards.ToImmutableList(), Id),
                Controller.Controller
            );
        }
    }
}
