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
    }
}
