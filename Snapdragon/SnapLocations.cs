using System.Collections.Immutable;
using Snapdragon.CardEffectEventBuilders;
using Snapdragon.GameFilters;
using Snapdragon.LocationFilters;
using Snapdragon.OngoingAbilities;
using Snapdragon.RevealAbilities;
using Snapdragon.SideFilters;
using Snapdragon.TargetFilters;
using Snapdragon.TriggeredAbilities;
using Snapdragon.TriggeredEffects;

namespace Snapdragon
{
    /// <summary>
    /// A collection of all of the defined locations for the actual game.
    /// </summary>
    public static class SnapLocations
    {
        public static readonly ImmutableList<LocationDefinition> All = new List<LocationDefinition>
        {
            new LocationDefinition(
                "Camp Lehigh",
                new AddRandomCardToHands(new CardDefinitionFilters.CardsWithCost(3))
            ),
            new LocationDefinition(
                "Central Park",
                new AddCardsToLocations<Location>(
                    new CardDefinition("Squirrel", 1, 1),
                    new AllLocations(),
                    new BothSides()
                )
            ),
            new(
                "Cloning Vats",
                null,
                null,
                new OnCardRevealedHere(new AddCopyOfCardToHand(new SameSide()))
            ),
            new("Death's Domain", null, null, new OnCardRevealedHere(new DestroyCardInPlay())),
            new LocationDefinition(
                "Jotunheim",
                null,
                null,
                new OnTurnEnd<Location>(new AddPowerToCardsHere(-1))
            ),
            new(
                "Kyln",
                null,
                new OngoingBlockLocationEffect<Location>(
                    EffectType.PlayCard,
                    new CardsHere(),
                    null,
                    new AfterTurn(4)
                )
            ),
            new(
                "Machineworld",
                null,
                null,
                new OnCardRevealedHere(new AddCopyOfCardToHand(new OtherSide()))
            ),
            new LocationDefinition(
                "Muir Island",
                null,
                null,
                new OnTurnEnd<Location>(new AddPowerToCardsHere(1))
            ),
            new LocationDefinition(
                "Murderworld",
                null,
                null,
                new OnSpecificTurnEnd<Location>(3, new DestroyCardsHere())
            ),
            new LocationDefinition(
                "Necrosha",
                null,
                new OngoingAdjustPower<Location>(new CardsHere(), -2)
            ),
            new LocationDefinition(
                "Negative Zone",
                null,
                new OngoingAdjustPower<Location>(new CardsHere(), -3)
            ),
            new LocationDefinition(
                "Nidavellir",
                null,
                new OngoingAdjustPower<Location>(new CardsHere(), 5)
            ),
            new LocationDefinition("Ruins"),
            new LocationDefinition(
                "Sanctum Sanctorum",
                null,
                new OngoingBlockLocationEffect<Location>(EffectType.PlayCard, new CardsHere())
            ),
            new LocationDefinition(
                "Sewer System",
                null,
                new OngoingAdjustPower<Location>(new CardsHere(), -1)
            ),
            new LocationDefinition(
                "Xandar",
                null,
                new OngoingAdjustPower<Location>(new CardsHere(), 1)
            ),
        }
            .OrderBy(l => l.Name)
            .ToImmutableList();

        public static readonly ImmutableDictionary<string, LocationDefinition> ByName =
            All.ToImmutableDictionary(ld => ld.Name);
    }
}
