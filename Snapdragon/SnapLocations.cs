using System.Collections.Immutable;
using Snapdragon.CardDefinitionFilters;
using Snapdragon.CardEffectEventBuilders;
using Snapdragon.LocationFilters;
using Snapdragon.RevealAbilities;
using Snapdragon.SideFilters;
using Snapdragon.TriggeredAbilities;

namespace Snapdragon
{
    /// <summary>
    /// A collection of all of the defined locations for the actual game.
    /// </summary>
    public static class SnapLocations
    {
        public static ImmutableList<LocationDefinition> All = new List<LocationDefinition>
        {
            new LocationDefinition("Camp Lehigh", new AddRandomCardToHands(new CardsWithCost(3))),
            new LocationDefinition(
                "Central Park",
                new AddCardsToLocations<Location>(
                    new CardDefinition("Squirrel", 1, 1),
                    new AllLocations<Location>(),
                    new BothSides<Location>()
                )
            ),
            new("Death's Domain", null, null, new OnCardRevealedHere(new DestroyCardInPlay())),
            new LocationDefinition("Ruins")
        }
            .OrderBy(l => l.Name)
            .ToImmutableList();

        public static ImmutableDictionary<string, LocationDefinition> ByName =
            All.ToImmutableDictionary(ld => ld.Name);
    }
}
