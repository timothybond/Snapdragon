namespace Snapdragon.GeneticAlgorithm
{
    public interface IGeneSequence<T>
        where T : IGeneSequence<T>
    {
        T Cross(T other);
    }
}
