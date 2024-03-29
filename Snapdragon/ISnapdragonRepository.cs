﻿using Snapdragon.GeneticAlgorithm;

namespace Snapdragon
{
    public interface ISnapdragonRepository : IDisposable
    {
        /// <summary>
        /// Creates or updates an <see cref="Experiment"/>.
        /// </summary>
        Task SaveExperiment(Experiment experiment);

        /// <summary>
        /// Gets an <see cref="Experiment"/> by its unique identifier.
        /// </summary>
        Task<Experiment?> GetExperiment(Guid id);

        /// <summary>
        /// Gets all <see cref="Experiment"/>s.
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<Experiment>> GetExperiments();

        /// <summary>
        /// Deletes an <see cref="Experiment"/> by its unique identifier.
        ///
        /// For idempotency, does not throw an error if the <see cref="Experiment"/> does not exist.
        /// </summary>
        Task DeleteExperiment(Guid id);

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
        Task SavePopulation(Population population);

        /// <summary>
        /// Gets the <see cref="Population"/> by unique identifier.
        ///
        /// Note that this will NOT include the <see cref="Population.Items"/> entries.
        /// </summary>
        Task<Population?> GetPopulation(Guid id);

        /// <summary>
        /// Gets the counts of all cards in the <see cref="Population"/> over all of its generations.
        ///
        /// Returns <c>null</c> if the population is not found.
        /// </summary>
        Task<IReadOnlyList<CardCount>?> GetCardCounts(Guid populationId);

        /// <summary>
        /// Gets the <see cref="Population"/>s associated with an <see cref="Experiment"/> by its unique identifier.
        ///
        /// Note that this will NOT include the <see cref="Population.Items"/> entries.
        /// </summary>
        Task<IReadOnlyList<Population>> GetPopulations(Guid experimentId);

        /// <summary>
        /// Gets the <see cref="Population"/> by <see cref="Experiment"/> unique identifier
        /// and generation number.
        ///
        /// Note that this will NOT include the <see cref="Population.Items"/> entries.
        /// </summary>
        Task<Population?> GetPopulation(Guid experimentId, int generation);

        /// <summary>
        /// Deletes the <see cref="Population"/> by unique identifier.
        ///
        /// For idempotency, does not throw an error if the <see cref="Experiment"/> does not exist.
        /// </summary>
        Task DeletePopulation(Guid id);

        Task SaveItem(GeneSequence item);

        /// <summary>
        /// Links together a gene sequence with a <see cref="Population"/>.
        /// </summary>
        Task AddItemToPopulation(Guid itemId, Guid populationId, int generation);

        /// <summary>
        /// Removes the link between a gene sequence with a <see cref="Population"/>.
        /// </summary>
        Task RemoveItemFromPopulation(Guid itemId, Guid populationId, int generation);

        /// <summary>
        /// Gets a single gene sequence by unique identifier.
        /// </summary>
        Task<GeneSequence?> GetItem(Guid id);

        /// <summary>
        /// Gets all members of a <see cref="Population"/> by the unique identifier
        /// of that <see cref="Population{T}"/> and the generation number.
        /// </summary>
        Task<IReadOnlyList<GeneSequence>> GetItems(
            Guid populationId,
            int generation
        );

        /// <summary>
        /// Deletes a member of a <see cref="Population{T}"/> by unique identifier.
        /// </summary>
        Task DeleteItem(Guid id);

        /// <summary>
        /// Saves a <see cref="CardDefinition"/>.
        ///
        /// Note that the repository is only obligated to store the <see cref="CardDefinition.Name"/>,
        /// <see cref="CardDefinition.Cost"/>, and <see cref="CardDefinition.Power"/> properties.
        /// </summary>
        Task SaveCardDefinition(CardDefinition cardDefinition);

        /// <summary>
        /// Gets a <see cref="CardDefinition"/>, by name.
        ///
        /// Note that the repository is only obligated to store the <see cref="CardDefinition.Name"/>,
        /// <see cref="CardDefinition.Cost"/>, and <see cref="CardDefinition.Power"/> properties.
        /// </summary>
        Task<CardDefinition?> GetCardDefinition(string cardName);

        /// <summary>
        /// Gets all <see cref="CardDefinition"/>.
        ///
        /// Note that the repository is only obligated to store the <see cref="CardDefinition.Name"/>,
        /// <see cref="CardDefinition.Cost"/>, and <see cref="CardDefinition.Power"/> properties.
        /// </summary>
        Task<IReadOnlyList<CardDefinition>> GetCardDefinitions();

        /// <summary>
        /// Deletes a <see cref="CardDefinition"/>, by name.
        ///
        /// For idempotency, does not throw an error if the <see cref="CardDefinition"/> does not exist.
        /// </summary>
        Task DeleteCardDefinition(string cardName);

        /// <summary>
        /// Saves an entry for a <see cref="Game"/>, which can then be logged against.
        /// </summary>
        Task SaveGame(GameRecord gameRecord);

        /// <summary>
        /// Gets the <see cref="GameRecord"/>s of all games played in a given <see cref="Experiment"/>
        /// during a given generation.
        /// </summary>
        Task<IReadOnlyList<GameRecord>> GetGames(Guid experimentId, int generation);

        /// <summary>
        /// Gets an entry for a <see cref="Game"/>.
        /// </summary>
        Task<GameRecord?> GetGame(Guid id);

        /// <summary>
        /// Deletes an entry for a <see cref="Game"/> and all of its associated logs.
        /// </summary>
        Task DeleteGame(Guid id);

        /// <summary>
        /// Saves a single log for a <see cref="Game"/>.
        ///
        /// Note that unlike most other Save methods on this interface,
        /// this is insert-only, and attempts to perform updates will fail.
        ///
        /// Uniqueness is enforced by the combination of the game's
        /// unique identifier and the "order" value on the logs.
        /// </summary>
        Task SaveGameLog(GameLogRecord log);

        /// <summary>
        /// Gets all logs for a <see cref="Game"/>, in order.
        /// </summary>
        Task<IReadOnlyList<GameLogRecord>> GetGameLogs(Guid gameId);
    }
}
