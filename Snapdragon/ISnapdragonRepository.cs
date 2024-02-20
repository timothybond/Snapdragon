using Snapdragon.GeneticAlgorithm;

namespace Snapdragon
{
    public interface ISnapdragonRepository
    {
        /// <summary>
        /// Creates or updates an <see cref="Experiment"/>.
        /// </summary>
        void SaveExperiment(Experiment experiment);

        /// <summary>
        /// Gets an <see cref="Experiment"/> by its unique identifier.
        /// </summary>
        Experiment? GetExperiment(Guid id);

        /// <summary>
        /// Gets all <see cref="Experiment"/>s.
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<Experiment> GetExperiments();

        /// <summary>
        /// Deletes an <see cref="Experiment"/> by its unique identifier.
        ///
        /// For idempotency, does not throw an error if the <see cref="Experiment"/> does not exist.
        /// </summary>
        void DeleteExperiment(Guid id);

        // Note: some of these methods are a bit clumsy because the consumer will
        // need to know in advance what some of the types are. At some point
        // I may rework things to solve for this. It probably won't surprise
        // future observers to learn I didn't originally plan to write the
        // database/frontend layer, and decided to do so after I'd already ran
        // some experiments and got curious about how the results came to be.

        /// <summary>
        /// Creatse or updates a top-level entry for the <see cref="Population{T}"/>.
        ///
        /// Note that this will NOT include the <see cref="Population{T}.Items"/> entries.
        /// </summary>
        void SavePopulation<T>(Population<T> population)
            where T : IGeneSequence<T>;

        /// <summary>
        /// Gets the <see cref="Population{T}"/> by unique identifier.
        ///
        /// Note that this will NOT include the <see cref="Population{T}.Items"/> entries.
        /// </summary>
        Population<T>? GetPopulation<T>(Guid id)
            where T : IGeneSequence<T>;

        /// <summary>
        /// Gets the <see cref="Population{T}"/> by <see cref="Experiment"/> unique identifier
        /// and generation number.
        ///
        /// Note that this will NOT include the <see cref="Population{T}.Items"/> entries.
        /// </summary>
        Population<T>? GetPopulation<T>(Guid experimentId, int generation)
            where T : IGeneSequence<T>;

        /// <summary>
        /// Deletes the <see cref="Population{T}"/> by unique identifier.
        ///
        /// For idempotency, does not throw an error if the <see cref="Experiment"/> does not exist.
        /// </summary>
        void DeletePopulation(Guid id);

        /// <summary>
        /// Gets a member of a <see cref="Population{T}"/> by unique identifier.
        /// </summary>
        IGeneSequence<T>? GetItem<T>(Guid id)
            where T : IGeneSequence<T>;

        /// <summary>
        /// Gets all members of a <see cref="Population{T}"/> by the unique identifier
        /// of that <see cref="Population{T}"/>.
        /// </summary>
        IReadOnlyList<IGeneSequence<T>> GetItems<T>(Guid populationId)
            where T : IGeneSequence<T>;

        /// <summary>
        /// Gets all members of a <see cref="Population{T}"/> by
        /// <see cref="Experiment"/> unique identifier and generation number.
        /// </summary>
        IReadOnlyList<IGeneSequence<T>> GetItems<T>(Guid experimentId, int generation)
            where T : IGeneSequence<T>;

        /// <summary>
        /// Saves a <see cref="CardDefinition"/>.
        ///
        /// Note that the repository is only obligated to store the <see cref="CardDefinition.Name"/>,
        /// <see cref="CardDefinition.Cost"/>, and <see cref="CardDefinition.Power"/> properties.
        /// </summary>
        void SaveCardDefinition(CardDefinition cardDefinition);

        /// <summary>
        /// Gets a <see cref="CardDefinition"/>, by name.
        ///
        /// Note that the repository is only obligated to store the <see cref="CardDefinition.Name"/>,
        /// <see cref="CardDefinition.Cost"/>, and <see cref="CardDefinition.Power"/> properties.
        /// </summary>
        CardDefinition? GetCardDefinition(string cardName);

        /// <summary>
        /// Gets all <see cref="CardDefinition"/>.
        ///
        /// Note that the repository is only obligated to store the <see cref="CardDefinition.Name"/>,
        /// <see cref="CardDefinition.Cost"/>, and <see cref="CardDefinition.Power"/> properties.
        /// </summary>
        IReadOnlyList<CardDefinition> GetCardDefinitions();

        /// <summary>
        /// Deletes a <see cref="CardDefinition"/>, by name.
        ///
        /// For idempotency, does not throw an error if the <see cref="CardDefinition"/> does not exist.
        /// </summary>
        void DeleteCardDefinition(string cardName);
    }
}
