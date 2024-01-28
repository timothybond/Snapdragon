using Snapdragon.CardAbilities;
using Snapdragon.CardConditions;

namespace Snapdragon
{
    /// <summary>
    /// A collection of all of the defined cards for the actual game.
    /// </summary>
    public static class SnapCards
    {
        public static IReadOnlyList<CardDefinition> All = new List<CardDefinition>
        {
            new("Misty Knight", 1, 2),
            new(
                "Rocket Raccoon",
                1,
                2,
                new OnRevealIf(new OpponentPlayedSameTurn(), new AddPowerSelf(2))
            ),
            new("Shocker", 2, 3),
            new(
                "Star-Lord",
                2,
                2,
                new OnRevealIf(new OpponentPlayedSameTurn(), new AddPowerSelf(3))
            ),
            new("Gamora", 5, 7, new OnRevealIf(new OpponentPlayedSameTurn(), new AddPowerSelf(5)))
        };

        public static IReadOnlyDictionary<string, CardDefinition> ByName = All.ToDictionary(cd =>
            cd.Name
        );
    }
}
