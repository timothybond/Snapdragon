using System.Collections.Immutable;
using Snapdragon.CardEffectEventBuilders;
using Snapdragon.Events;
using Snapdragon.Fluent;
using Snapdragon.Fluent.EffectBuilders;
using Snapdragon.Fluent.Filters;
using Snapdragon.Fluent.Selectors;
using Snapdragon.TriggeredAbilities;
using Snapdragon.TriggeredEffects;

namespace Snapdragon
{
    /// <summary>
    /// A collection of all of the defined locations for the actual game.
    /// </summary>
    public static class SnapLocations
    {
        /// <summary>
        /// Convenience reference for the builder for <see cref="OnReveal{Card}"/> abilities.
        /// </summary>
        public static Fluent.LocationRevealed OnReveal => new Fluent.LocationRevealed();

        /// <summary>
        /// Convenience reference for the builder for <see cref="Ongoing{Card}"/> abilities.
        /// </summary>
        public static Fluent.LocationOngoing Ongoing => new Fluent.LocationOngoing();

        public static HereFilter Here => new HereFilter();

        public static SnapCardsSelector PossibleCards => new SnapCardsSelector();

        public static readonly ImmutableList<LocationDefinition> All = new List<LocationDefinition>
        {
            new LocationDefinition(
                "Camp Lehigh",
                OnReveal.Build(new BothSides().AddToHand(PossibleCards.WithCost(3).GetRandom()))
            ),
            new LocationDefinition(
                "Central Park",
                OnReveal.Build(
                    new AllLocations().AddCard(
                        new CardDefinition("Squirrel", 1, 1),
                        new BothSides()
                    )
                )
            ),
            new(
                "Cloning Vats",
                null,
                null,
                When.RevealedAnd<CardRevealedEvent>()
                    .Build(EventCard.Get.CopyToHand(EventCard.Player))
            ),
            new("Death's Domain", null, null, new OnCardRevealedHere(new DestroyCardInPlay())),
            new LocationDefinition(
                "Jotunheim",
                null,
                null,
                new OnTurnEnd<Location>(new AddPowerToCardsHere(-1))
            ),
            new("Kyln", null, Ongoing.If.AfterTurn(4).Block(EffectType.PlayCard).ForLocation(Here)),
            new(
                "Machineworld",
                null,
                null,
                When.RevealedAnd<CardRevealedEvent>()
                    .Build(EventCard.Get.CopyToHand(EventCard.Player.Other()))
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
                Ongoing.AdjustPower(new RevealedCards().AtLocation(), -2)
            ),
            new LocationDefinition(
                "Negative Zone",
                null,
                Ongoing.AdjustPower(new RevealedCards().AtLocation(), -3)
            ),
            new LocationDefinition(
                "Nidavellir",
                null,
                Ongoing.AdjustPower(new RevealedCards().AtLocation(), 5)
            ),
            new LocationDefinition("Ruins"),
            new LocationDefinition(
                "Sanctum Sanctorum",
                null,
                Ongoing.Block(EffectType.PlayCard).ForLocation(Here)
            ),
            new LocationDefinition(
                "Sewer System",
                null,
                Ongoing.AdjustPower(new RevealedCards().AtLocation(), -1)
            ),
            new LocationDefinition(
                "Xandar",
                null,
                Ongoing.AdjustPower(new RevealedCards().AtLocation(), 1)
            ),
        }
            .OrderBy(l => l.Name)
            .ToImmutableList();

        public static readonly ImmutableDictionary<string, LocationDefinition> ByName =
            All.ToImmutableDictionary(ld => ld.Name);
    }
}
