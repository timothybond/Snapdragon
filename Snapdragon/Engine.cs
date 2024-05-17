using System.Collections.Immutable;

namespace Snapdragon
{
    public class Engine
    {
        private readonly IGameLogger logger;

        public Engine(IGameLogger logger)
        {
            this.logger = logger;
        }

        public Game CreateGame(
            PlayerConfiguration topPlayerConfig,
            PlayerConfiguration bottomPlayerConfig,
            bool shuffle = true,
            Side? firstRevealed = null,
            ISnapdragonRepository? repository = null,
            Guid? experimentId = null,
            int? generation = null,
            string? leftLocationName = null,
            string? middleLocationName = null,
            string? rightLocationName = null
        )
        {
            var locationDefinitions = GetLocations(
                leftLocationName,
                middleLocationName,
                rightLocationName
            );

            var firstRevealedOrRandom = firstRevealed ?? Random.Side();

            // TODO: Specify different Locations, with effects
            // TODO: Handle card abilities that put them in a specific draw order
            var game = new Game(
                Guid.NewGuid(),
                0,
                new Location(Column.Left, locationDefinitions.Left),
                new Location(Column.Middle, locationDefinitions.Middle),
                new Location(Column.Right, locationDefinitions.Right),
                topPlayerConfig.ToPlayer(Side.Top, shuffle),
                bottomPlayerConfig.ToPlayer(Side.Bottom, shuffle),
                firstRevealedOrRandom,
                [],
                [],
                ImmutableDictionary<long, ICardInstance>.Empty,
                ImmutableDictionary<long, EventType>.Empty,
                ImmutableDictionary<EventType, ImmutableHashSet<long>>.Empty,
                ImmutableDictionary<long, Sensor<ICard>>.Empty,
                ImmutableDictionary<long, EventType>.Empty,
                ImmutableDictionary<EventType, ImmutableHashSet<long>>.Empty,
                this.logger
            ).Initialize();

            if (repository != null)
            {
                game = game with
                {
                    Logger = new RepositoryGameLogger(repository, game.Id, experimentId, generation)
                };
            }

            game = game.DrawCard(Side.Top)
                .DrawCard(Side.Top)
                .DrawCard(Side.Top)
                .DrawCard(Side.Bottom)
                .DrawCard(Side.Bottom)
                .DrawCard(Side.Bottom);

            return game;
        }

        private (
            LocationDefinition Left,
            LocationDefinition Middle,
            LocationDefinition Right
        ) GetLocations(
            string? leftLocationName = null,
            string? middleLocationName = null,
            string? rightLocationName = null
        )
        {
            var specifiedNames = new string?[]
            {
                leftLocationName,
                middleLocationName,
                rightLocationName
            }
                .Where(n => n != null)
                .ToList<string>();

            var remainingNames = SnapLocations.ByName.Keys.Where(k => !specifiedNames.Contains(k));
            var remainingLocations = remainingNames.Select(n => SnapLocations.ByName[n]).ToList();

            // TODO: Clean this up once we know we have plenty of locations -
            // right now it is here because there might not be three implemented ones.
            while (remainingLocations.Count < 3)
            {
                remainingLocations.Add(SnapLocations.ByName["Ruins"]);
            }

            remainingLocations = remainingLocations.OrderBy(ld => Random.Next()).ToList();

            return (
                GetLocationOrDefault(leftLocationName, remainingLocations[0]),
                GetLocationOrDefault(middleLocationName, remainingLocations[1]),
                GetLocationOrDefault(rightLocationName, remainingLocations[2])
            );
        }

        private LocationDefinition GetLocationOrDefault(
            string? locationName,
            LocationDefinition defaultLocation
        )
        {
            if (locationName != null)
            {
                return SnapLocations.ByName[locationName];
            }

            return defaultLocation;
        }
    }
}
