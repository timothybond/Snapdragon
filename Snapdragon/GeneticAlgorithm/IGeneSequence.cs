namespace Snapdragon.GeneticAlgorithm
{
    public interface IGeneSequence
    {
        PlayerConfiguration GetPlayerConfiguration(int index);
    }

    public interface IGeneSequence<T> : IGeneSequence
        where T : IGeneSequence<T>
    {
        T Cross(T other);
    }
}
