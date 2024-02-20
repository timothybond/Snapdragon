namespace Snapdragon.GeneticAlgorithm
{
    public interface IGeneSequence
    {
        PlayerConfiguration GetPlayerConfiguration();

        IReadOnlyList<CardDefinition> GetCards();
    }

    public interface IGeneSequence<T> : IGeneSequence
        where T : IGeneSequence<T>
    {
        T Cross(T other);
    }
}
