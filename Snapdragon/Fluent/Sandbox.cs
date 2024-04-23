using Snapdragon.Events;
using Snapdragon.Fluent.EffectBuilders;
using Snapdragon.Fluent.Selectors;
using Snapdragon.Fluent.Transforms;

namespace Snapdragon.Fluent
{
    public static class Sandbox
    {
        public static void Test()
        {
            var medusaOnReveal = new CardRevealed()
                .If.InColumn(Column.Middle)
                .Then(new Self().ModifyPower(3));

            var rocketRaccoonOnReveal = new CardRevealed()
                .If.PastEvent()
                .OfType<CardRevealedEvent>()
                .Where(EventCard.SameSide.And(EventCard.Here))
                .Then(new Self().ModifyPower(2));

            var ironheartOnReveal = new CardRevealed().ModifyPower(My.Cards.GetRandom(3), 2);

            var ladySifOnReveal = new CardRevealed().Discard(My.Hand.WithMaxCost().GetRandom());
            var swordMasterOnReveal = new CardRevealed().Discard(My.Hand.GetRandom());
            var colleenWingOnReveal = new CardRevealed().Discard(My.Hand.WithMinCost().GetRandom());
            var bladeOnReveal = new CardRevealed().Discard(My.Hand.Last());
            var modokOnReveal = new CardRevealed().Discard(My.Hand);

            ISelector<ICardInstance, ICardInstance> selector = new Self();

            var humanTorchTrigger = When.InPlayAnd<CardMovedEvent>()
                .Where(EventCard.Self)
                .Then(new Self().DoublePower());

            var vultureTrigger = When.InPlayAnd<CardMovedEvent>()
                .Where(EventCard.Self)
                .Then(new Self().ModifyPower(5));

            var bishopTrigger = When.InPlayAnd<CardRevealedEvent>()
                .Where(EventCard.SameSide)
                .Then(new Self().ModifyPower(1));

            var angelaTrigger = When.InPlayAnd<CardRevealedEvent>()
                .Where(EventCard.SameSide.And(EventCard.Here))
                .Then(new Self().ModifyPower(1));

            var kravenTrigger = When.InPlayAnd<CardMovedEvent>()
                .Where(Moved.ToHere)
                .Then(new Self().ModifyPower(2));

            var apocalypseTrigger = When.Discarded.Then(
                new Self().ModifyPower(4).And(new Self().ReturnToHand())
            );

            var multipleManTrigger = When.InPlayAnd<CardMovedEvent>()
                .Where(EventCard.Self)
                .Then(new Self().CopyToLocation(At.PriorLocation));

            var swarmTrigger = When.Discarded.Then(
                new Self().CopyToHand(new WithZeroCost()).Times(2)
            );

            var antManOngoing = new CardOngoing().If.LocationFull().AdjustPower(new Self(), 3);

            var blueMarvelOngoing = new CardOngoing().AdjustPower(My.OtherCards, 1);

            var kaZarOngoing = new CardOngoing().AdjustPower(My.OtherCards.WithCost(1), 1);

            var klawOngoing = new CardOngoing().AdjustLocationPower(new LocationToTheRight(), 6);

            var misterFantasticOngoing = new CardOngoing().AdjustLocationPower(
                new AdjacentLocations(),
                2
            );

            var armorOngoing = new CardOngoing().Block(EffectType.DestroyCard).ForLocation(At.Here);

            var ebonyMawOngoing = new CardOngoing()
                .Block(EffectType.PlayCard)
                .ForLocationAndSide(At.Here, new SameSide());

            var rescueOnReveal = If.NextTurnEvent<CardPlayedEvent>()
                .Where(EventCard.AtSensor.And(EventCard.SameSide))
                .ModifyPower(new SourceCard(), 4);

            var gamoraOnReveal = If.NoNextTurnEvent<CardPlayedEvent>()
                .Where(EventCard.AtSensor.And(EventCard.SameSide))
                .ModifyPower(new SourceCard(), 4);

            var revealedCardsHere = new RevealedCards().AtLocation();

            var cloningVatsTrigger = When.RevealedAnd<CardRevealedEvent>()
                .Then(EventCard.Get.CopyToHand());
        }
    }
}
