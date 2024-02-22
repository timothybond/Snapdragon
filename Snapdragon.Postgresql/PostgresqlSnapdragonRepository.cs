﻿using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Dapper;
using Npgsql;
using Snapdragon.CardOrders;
using Snapdragon.GeneticAlgorithm;

namespace Snapdragon.Postgresql
{
    public class PostgresqlSnapdragonRepository : ISnapdragonRepository, IDisposable
    {
        private readonly string _connectionString;
        private readonly NpgsqlDataSource _dataSource;
        private NpgsqlConnection? _connection = null;
        private bool disposed = false;

        public PostgresqlSnapdragonRepository(string connectionString)
        {
            _connectionString = connectionString;
            _dataSource = NpgsqlDataSource.Create(connectionString);
        }

        #region Experiments

        public async Task SaveExperiment(Experiment experiment)
        {
            var experimentRow = new Data.Experiment
            {
                Id = experiment.Id,
                Name = experiment.Name,
                Created = experiment.Started
            };

            var conn = GetConnection();

            await conn.ExecuteAsync(
                "INSERT INTO experiment (id, name, created) VALUES (@Id, @Name, @Created) "
                    + "ON CONFLICT(id) DO UPDATE SET name = EXCLUDED.name, created = EXCLUDED.created;",
                experimentRow
            );
        }

        public async Task<Experiment?> GetExperiment(Guid id)
        {
            var conn = GetConnection();

            var row = await conn.QueryFirstOrDefaultAsync<Data.Experiment>(
                "SELECT Id, Name, Created FROM experiment WHERE Id = @Id",
                new { Id = id }
            );

            if (row == null)
            {
                return null;
            }

            return new Experiment(row.Id, row.Name, row.Created);
        }

        public async Task<IReadOnlyList<Experiment>> GetExperiments()
        {
            var conn = GetConnection();

            var rows = await conn.QueryAsync<Data.Experiment>(
                "SELECT Id, Name, Created FROM experiment"
            );

            return rows.Select(r => new Experiment(r.Id, r.Name, r.Created)).ToList();
        }

        public async Task DeleteExperiment(Guid id)
        {
            var conn = GetConnection();

            await conn.ExecuteAsync("DELETE FROM experiment WHERE id = @Id", new { Id = id });
        }

        #endregion

        #region Populations

        public async Task SavePopulation<T>(Population<T> population)
            where T : IGeneSequence<T>
        {
            var conn = GetConnection();

            var fixedCards = new HashSet<string>(
                population.Genetics.GetFixedCards().Select(c => c.Name)
            );
            var otherCards = new HashSet<string>(
                population
                    .Genetics.AllPossibleCards.Select(c => c.Name)
                    .Where(n => !fixedCards.Contains(n))
            );

            using (var transaction = await conn.BeginTransactionAsync())
            {
                try
                {
                    var row = Data.Population.FromPopulation(population);
                    await conn.ExecuteAsync(
                        "INSERT INTO population (id, experimentid, name, controller, mutationper, orderby)"
                            + " VALUES (@Id, @ExperimentId, @Name, @Controller, @MutationPer, @OrderBy) "
                            + "ON CONFLICT(id) DO UPDATE SET "
                            + "experimentid = EXCLUDED.experimentid, "
                            + "name = EXCLUDED.name, "
                            + "controller = EXCLUDED.controller, "
                            + "mutationper = EXCLUDED.mutationper, "
                            + "orderby = EXCLUDED.orderby;",
                        row,
                        transaction
                    );

                    // TODO: Remove old card associations
                    await conn.ExecuteAsync(
                        "DELETE FROM population_carddefinition WHERE populationid = @Id",
                        row,
                        transaction
                    );

                    foreach (var fixedCard in fixedCards)
                    {
                        await conn.ExecuteAsync(
                            "INSERT INTO population_carddefinition (populationid, carddefinitionname, fixed) "
                                + "VALUES (@PopulationId, @CardDefinitionName, true)",
                            new { PopulationId = population.Id, CardDefinitionName = fixedCard },
                            transaction
                        );
                    }

                    foreach (var otherCard in otherCards)
                    {
                        await conn.ExecuteAsync(
                            "INSERT INTO population_carddefinition (populationid, carddefinitionname, fixed) "
                                + "VALUES (@PopulationId, @CardDefinitionName, false)",
                            new { PopulationId = population.Id, CardDefinitionName = otherCard },
                            transaction
                        );
                    }

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<Population<T>?> GetPopulation<T>(Guid id)
            where T : IGeneSequence<T>
        {
            var conn = GetConnection();

            // TODO: Decide whether this needs to be wrapped in a transaction or something

            // TODO: Make this altogether less messy (this would probably require refactoring
            // the Genetics/Population classes to unify them rather than having the generics)
            var row = await conn.QueryFirstOrDefaultAsync<Data.Population>(
                "SELECT id, experimentid, name, controller, mutationper, orderby FROM population WHERE id = @Id",
                new { Id = id }
            );

            return await GetPopulation<T>(row, conn);
        }

        public async Task<Population<T>?> GetPopulation<T>(Guid experimentId, int generation)
            where T : IGeneSequence<T>
        {
            var conn = GetConnection();

            // TODO: Decide whether this needs to be wrapped in a transaction or something

            // TODO: Make this altogether less messy (this would probably require refactoring
            // the Genetics/Population classes to unify them rather than having the generics)
            var row = await conn.QueryFirstOrDefaultAsync<Data.Population>(
                "SELECT id, experimentid, generation, name, controller, mutationper, orderby FROM population WHERE experimentid = @ExperimentId AND generation = @Generation",
                new { ExperimentId = experimentId, Generation = generation }
            );

            return await GetPopulation<T>(row, conn);
        }

        private async Task<Population<T>?> GetPopulation<T>(
            Data.Population? row,
            NpgsqlConnection conn
        )
            where T : IGeneSequence<T>
        {
            if (row == null)
            {
                return null;
            }

            var cards = await conn.QueryAsync<Data.PopulationCardDefinition>(
                "SELECT populationid, carddefinitionname, fixed FROM population_carddefinition WHERE populationid = @Id",
                new { Id = row.Id }
            );

            var fixedCards = cards
                .Where(c => c.Fixed)
                .Select(c => SnapCards.ByName[c.CardDefinitionName])
                .ToImmutableList();
            var allCards = cards
                .Select(c => SnapCards.ByName[c.CardDefinitionName])
                .ToImmutableList();

            var controller = ParseController(row.Controller);
            var orderBy = ParseOrderBy(row.OrderBy);

            if (fixedCards.Count > 0)
            {
                var genetics = new PartiallyFixedGenetics(
                    fixedCards,
                    allCards,
                    controller,
                    row.MutationPer,
                    orderBy
                );

                var population = new Population<PartiallyFixedCardGeneSequence>(
                    genetics,
                    0,
                    row.Name,
                    row.ExperimentId
                );

                if (typeof(PartiallyFixedCardGeneSequence) != typeof(T))
                {
                    throw new InvalidOperationException(
                        "Population has fixed cards, but was not requested as a PartiallyFixedCardGeneSequence population."
                    );
                }

                return population as Population<T>;
            }
            else
            {
                var genetics = new CardGenetics(allCards, controller, row.MutationPer, orderBy);

                var population = new Population<CardGeneSequence>(
                    genetics,
                    0,
                    row.Name,
                    row.ExperimentId
                );

                if (typeof(CardGeneSequence) != typeof(T))
                {
                    throw new InvalidOperationException(
                        "Population has no fixed cards, but was not requested as a CardGeneSequence population."
                    );
                }

                return population as Population<T>;
            }
        }

        public async Task DeletePopulation(Guid id)
        {
            var conn = GetConnection();

            using (var transaction = await conn.BeginTransactionAsync())
            {
                try
                {
                    await conn.ExecuteAsync(
                        "DELETE FROM population_carddefinition WHERE populationid = @Id",
                        new { Id = id }
                    );
                    await conn.ExecuteAsync(
                        "DELETE FROM population WHERE id = @Id",
                        new { Id = id }
                    );
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                }
            }
        }

        #endregion

        #region Items

        public async Task SaveItem<T>(IGeneSequence<T> item)
            where T : IGeneSequence<T>
        {
            var controller = item.GetControllerString() ?? string.Empty;

            var row = new Data.Item()
            {
                Id = item.Id,
                FirstParentId = item.FirstParentId,
                SecondParentId = item.SecondParentId,
                Controller = controller
            };

            var conn = GetConnection();

            using var transaction = await conn.BeginTransactionAsync();

            try
            {
                await conn.ExecuteAsync(
                    "INSERT INTO item (id, firstparentid, secondparentid, controller) "
                        + "VALUES (@Id, @FirstParentId, @SecondParentId, @Controller) "
                        + "ON CONFLICT(id) DO UPDATE SET "
                        + "firstparentid = EXCLUDED.firstparentid, "
                        + "secondparentid = EXCLUDED.secondparentid, "
                        + "controller = EXCLUDED.controller;",
                    row,
                    transaction
                );

                await conn.ExecuteAsync(
                    "DELETE FROM item_carddefinition WHERE itemid = @Id",
                    row,
                    transaction
                );

                var cardNames = item.GetCards().Select(c => c.Name).ToList();

                for (var cardOrder = 0; cardOrder < cardNames.Count; cardOrder++)
                {
                    var cardName = cardNames[cardOrder];

                    await conn.ExecuteAsync(
                        "INSERT INTO item_carddefinition (itemid, carddefinitionname, cardorder) VALUES (@Id, @Name, @CardOrder)",
                        new
                        {
                            Id = item.Id,
                            Name = cardName,
                            CardOrder = cardOrder
                        },
                        transaction
                    );
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task AddItemToPopulation(Guid itemId, Guid populationId, int generation)
        {
            var conn = GetConnection();

            await conn.ExecuteAsync(
                "INSERT INTO population_item (populationid, itemid, generation) "
                    + "VALUES (@PopulationId, @ItemId, @Generation)",
                new
                {
                    PopulationId = populationId,
                    ItemId = itemId,
                    Generation = generation
                }
            );
        }

        public async Task RemoveItemFromPopulation(Guid itemId, Guid populationId, int generation)
        {
            var conn = GetConnection();

            await conn.ExecuteAsync(
                "DELETE FROM population_item "
                    + "WHERE populationid = @PopulationId "
                    + "AND itemid = @ItemId "
                    + "AND generation = @Generation",
                new
                {
                    PopulationId = populationId,
                    ItemId = itemId,
                    Generation = generation
                }
            );
        }

        public async Task<T?> GetItem<T>(Guid id)
            where T : class, IGeneSequence<T>
        {
            var conn = GetConnection();

            var row = await conn.QueryFirstOrDefaultAsync<Data.Item>(
                "SELECT id, controller, firstparentid, secondparentid "
                    + "FROM item "
                    + "WHERE id = @Id",
                new { Id = id }
            );

            if (row == null)
            {
                return default(T);
            }

            var populationItem = await conn.QueryFirstOrDefaultAsync<Data.PopulationItem>(
                "SELECT populationid, itemid, generation FROM population_item WHERE itemid = @Id",
                new { id = id }
            );

            if (populationItem == null)
            {
                return await ToGeneSequence<T>(row, null, conn);
            }

            var population = await GetPopulation<T>(populationItem.PopulationId);

            return await ToGeneSequence(row, population, conn);
        }

        public async Task<IReadOnlyList<T>> GetItems<T>(Guid populationId, int generation)
            where T : class, IGeneSequence<T>
        {
            var conn = GetConnection();

            var population = await GetPopulation<T>(populationId);

            if (population == null)
            {
                return new List<T>();
            }

            var rows = await conn.QueryAsync<Data.Item>(
                "SELECT item.id, item.controller, item.firstparentid, item.secondparentid "
                    + "FROM item INNER JOIN population_item "
                    + "ON item.id = population_item.itemid "
                    + "WHERE population_item.populationid = @PopulationId",
                new { PopulationId = populationId }
            );

            return (
                await Task.WhenAll(rows.Select(r => ToGeneSequence(r, population, conn)))
            ).ToList();
        }

        public async Task DeleteItem(Guid id)
        {
            var conn = GetConnection();

            using var transaction = await conn.BeginTransactionAsync();

            try
            {
                await conn.ExecuteAsync(
                    "DELETE FROM item_carddefinition WHERE itemid = @Id",
                    new { Id = id },
                    transaction
                );
                await conn.ExecuteAsync(
                    "DELETE FROM population_item WHERE itemid = @Id",
                    new { Id = id },
                    transaction
                );
                await conn.ExecuteAsync(
                    "DELETE FROM item WHERE id = @Id",
                    new { Id = id },
                    transaction
                );
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        #endregion

        #region Card Definitions

        public async Task SaveCardDefinition(CardDefinition cardDefinition)
        {
            var row = (Data.CardDefinition)cardDefinition;

            var conn = GetConnection();

            await conn.ExecuteAsync(
                "INSERT INTO carddefinition (name, cost, power) VALUES (@Name, @Cost, @Power) "
                    + "ON CONFLICT(name) DO UPDATE SET cost = EXCLUDED.cost, power = EXCLUDED.power;",
                row
            );
        }

        public async Task<CardDefinition?> GetCardDefinition(string cardName)
        {
            var conn = GetConnection();

            var row = await conn.QueryFirstOrDefaultAsync<Data.CardDefinition>(
                "SELECT name, cost, power FROM carddefinition WHERE name = @Name",
                new { Name = cardName }
            );

            if (row == null)
            {
                return null;
            }

            return (Snapdragon.CardDefinition)row;
        }

        public async Task<IReadOnlyList<CardDefinition>> GetCardDefinitions()
        {
            var conn = GetConnection();

            var rows = await conn.QueryAsync<Data.CardDefinition>(
                "SELECT name, cost, power FROM carddefinition"
            );

            return rows.Select(cd => (CardDefinition)cd).ToList();
        }

        public async Task DeleteCardDefinition(string cardName)
        {
            var conn = GetConnection();

            await conn.ExecuteAsync(
                "DELETE FROM carddefinition WHERE name = @Name",
                new { Name = cardName }
            );
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Helper function for converting "item" rows into instances of <see cref="IGeneSequence{T}"/>.
        ///
        /// Because of the current oddities of dealing with generics, this will basically only work
        /// for instances of <see cref="PartiallyFixedCardGeneSequence"/> or <see cref="CardGeneSequence"/>.
        /// </summary>
        private async Task<T> ToGeneSequence<T>(
            Data.Item item,
            Population<T>? population,
            NpgsqlConnection conn
        )
            where T : class, IGeneSequence<T>
        {
            var controller = ParseController(item.Controller);

            var itemCards = (
                await conn.QueryAsync<Data.ItemCardDefinition>(
                    "SELECT itemid, carddefinitionname, cardorder FROM item_carddefinition WHERE itemid = @Id",
                    item
                )
            )
                .OrderBy(cd => cd.CardOrder)
                .Select(ic => SnapCards.ByName[ic.CardDefinitionName])
                .ToImmutableList();

            var fixedCardsForPopulation =
                population?.Genetics.GetFixedCards() ?? ImmutableList<CardDefinition>.Empty;

            if (fixedCardsForPopulation.Count > 0)
            {
                if (typeof(T) != typeof(PartiallyFixedCardGeneSequence))
                {
                    throw new InvalidOperationException(
                        "Item had fixed cards (based on its population) but was not requested as a PartiallyFixedCardGeneSequence"
                    );
                }

                var remainingCards = itemCards
                    .Where(c => !fixedCardsForPopulation.Contains(c))
                    .ToImmutableList();

                // All of the defaults for an unset population below are pretty mediocre
                var geneSequence = new PartiallyFixedCardGeneSequence(
                    new FixedCardGeneSequence(fixedCardsForPopulation, Guid.Empty, null, null),
                    new CardGeneSequence(
                        remainingCards,
                        population?.Genetics.AllPossibleCards ?? SnapCards.All,
                        Guid.Empty,
                        population?.Genetics.MutationPer ?? 200,
                        population?.Genetics.OrderBy ?? new RandomCardOrder(),
                        null,
                        null,
                        null
                    ),
                    controller,
                    item.Id,
                    item.FirstParentId,
                    item.SecondParentId
                );

                return (geneSequence as T)!;
            }
            else // No fixed cards
            {
                if (typeof(T) != typeof(CardGeneSequence))
                {
                    throw new InvalidOperationException(
                        "Item had no fixed cards (based on its population) but was not requested as a CardGeneSequence."
                    );
                }

                // All of the defaults for an unset population below are pretty mediocre
                var geneSequence = new CardGeneSequence(
                    itemCards,
                    population?.Genetics.AllPossibleCards ?? SnapCards.All,
                    item.Id,
                    population?.Genetics.MutationPer ?? 200,
                    population?.Genetics.OrderBy ?? new RandomCardOrder(),
                    item.FirstParentId,
                    item.SecondParentId,
                    controller
                );

                return (geneSequence as T)!;
            }
        }

        private NpgsqlConnection GetConnection()
        {
            // TODO: Handle thread safety if it ever ends up mattering
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(PostgresqlSnapdragonRepository));
            }

            _connection ??= _dataSource.OpenConnection();

            return _connection;
        }

        ICardOrder ParseOrderBy(string orderBy)
        {
            switch (orderBy)
            {
                case "Random":
                    return new RandomCardOrder();
                case "Existing":
                    return new ExistingCardOrder();
                default:
                    throw new NotImplementedException(
                        $"Unable to parse card-order-by specification: {orderBy}"
                    );
            }
        }

        IPlayerController ParseController(string controller)
        {
            if (string.Equals(controller, "Random"))
            {
                return new RandomPlayerController();
            }

            var monteCarloRegexMatch = Regex.Match(controller, "MonteCarlo\\(([0-9]+)\\)");

            if (monteCarloRegexMatch.Success)
            {
                var simulationCount = int.Parse(monteCarloRegexMatch.Groups[1].Value);
                return new MonteCarloSearchController(simulationCount);
            }

            throw new NotImplementedException(
                $"Unable to parse controller specification: {controller}"
            );
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                if (_connection != null)
                {
                    _connection.Dispose();
                    _connection = null;
                }

                _dataSource.Dispose();

                disposed = true;
            }
        }

        #endregion
    }
}
