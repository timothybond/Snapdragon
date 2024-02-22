namespace Snapdragon.GeneticAlgorithm
{
    public interface IGeneSequence
    {
        public Guid Id { get; }

        public Guid? FirstParentId { get; }

        public Guid? SecondParentId { get; }

        /// <summary>
        /// Gets the <see cref="PlayerConfiguration"/> used to play a game with this sequence.
        /// </summary>
        /// <returns></returns>
        PlayerConfiguration GetPlayerConfiguration();

        /// <summary>
        /// Gets the <see cref="CardDefinition"/>s used to play agame with this sequence.
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<CardDefinition> GetCards();

        /// <summary>
        /// Gets a string representation of the <see cref="IPlayerController"/> used here.
        /// </summary>
        string? GetControllerString();
    }

    public interface IGeneSequence<T> : IGeneSequence
        where T : IGeneSequence<T>
    {
        T Cross(T other);
    }
}
