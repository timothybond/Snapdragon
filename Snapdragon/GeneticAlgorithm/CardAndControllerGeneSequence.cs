using System.Collections.Immutable;

namespace Snapdragon.GeneticAlgorithm
{
    public record CardAndControllerGeneSequence(
        CardGeneSequence Cards,
        ControllerGeneSequence Controller
    ) : IGeneSequence<CardAndControllerGeneSequence>
    {
        public CardAndControllerGeneSequence Cross(CardAndControllerGeneSequence other)
        {
            return new CardAndControllerGeneSequence(
                this.Cards.Cross(other.Cards),
                this.Controller.Cross(other.Controller)
            );
        }

        public IReadOnlyList<CardDefinition> GetCards()
        {
            return this.Cards.Cards;
        }

        public PlayerConfiguration GetPlayerConfiguration(int index)
        {
            return new PlayerConfiguration(
                $"Deck {index}",
                new Deck(Cards.Cards.ToImmutableList()),
                Controller.Controller
            );
        }
    }
}
