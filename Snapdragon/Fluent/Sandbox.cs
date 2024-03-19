﻿using Snapdragon.Events;
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
                .ModifyPower(new Self(), 3);

            var rocketRaccoonOnReveal = new CardRevealed()
                .If.PastEvent.OfType<CardRevealedEvent>()
                .Where(EventCard.SameSide.And(EventCard.Here))
                .ModifyPower(new Self(), 2);

            var ironheartOnReveal = new CardRevealed().ModifyPower(My.Cards.GetRandom(3), 2);

            var ladySifOnReveal = new CardRevealed().Discard(My.Hand.WithMaxCost().GetRandom());
            var swordMasterOnReveal = new CardRevealed().Discard(My.Hand.GetRandom());
            var colleenWingOnReveal = new CardRevealed().Discard(My.Hand.WithMinCost().GetRandom());
            var bladeOnReveal = new CardRevealed().Discard(My.Hand.Last());
            var modokOnReveal = new CardRevealed().Discard(My.Hand);

            ICardSelector<ICard> selector = new Self();

            var humanTorchTrigger = When.InPlayAnd<CardMovedEvent>()
                .Where(EventCard.Self)
                .Build(new Self().DoublePower());

            var vultureTrigger = When.InPlayAnd<CardMovedEvent>()
                .Where(EventCard.Self)
                .Build(new Self().ModifyPower(5));

            var bishopTrigger = When.InPlayAnd<CardRevealedEvent>()
                .Where(EventCard.SameSide)
                .Build(new Self().ModifyPower(1));

            var angelaTrigger = When.InPlayAnd<CardRevealedEvent>()
                .Where(EventCard.SameSide.And(EventCard.Here))
                .Build(new Self().ModifyPower(1));

            var kravenTrigger = When.InPlayAnd<CardMovedEvent>()
                .Where(Moved.ToHere)
                .Build(new Self().ModifyPower(2));

            var apocalypseTrigger = When.Discarded.Build(
                new Self().ModifyPower(4).And(new Self().ReturnToHand())
            );

            var multipleManTrigger = When.InPlayAnd<CardMovedEvent>()
                .Where(EventCard.Self)
                .Build(new Self().CopyToLocation(At.PriorLocation));

            var swarmTrigger = When.Discarded.Build(
                new Self().CopyToHand(new WithZeroCost()).Times(2)
            );

            //var wolverineTrigger = When.DiscardedOrDestroyed.Build(new Self().ModifyPower(2).And())
        }
    }
}