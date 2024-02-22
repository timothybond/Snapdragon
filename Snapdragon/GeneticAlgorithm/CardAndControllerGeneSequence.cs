using System.Collections.Immutable;

namespace Snapdragon.GeneticAlgorithm
{
    public record CardAndControllerGeneSequence(
        CardGeneSequence Cards,
        ControllerGeneSequence Controller,
        Guid Id,
        Guid? FirstParentId,
        Guid? SecondParentId
    ) : IGeneSequence<CardAndControllerGeneSequence>
    {
        public CardAndControllerGeneSequence Cross(CardAndControllerGeneSequence other)
        {
            return new CardAndControllerGeneSequence(
                this.Cards.Cross(other.Cards),
                this.Controller.Cross(other.Controller),
                Guid.NewGuid(),
                this.Id,
                other.Id
            );
        }

        public IReadOnlyList<CardDefinition> GetCards()
        {
            return this.Cards.Cards;
        }

        public string GetControllerString()
        {
            return Controller.ToString();
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
