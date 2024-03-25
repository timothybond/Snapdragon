using Snapdragon.Fluent.Builders;
using Snapdragon.Fluent.EffectBuilders;
using Snapdragon.Fluent.EventFilters;

namespace Snapdragon.Fluent
{
    public static class NextTurn
    {
        public static OnReveal<Card> CanMoveHere()
        {
            return new CardRevealed().Build(
                new CreateMoveSensorBuilder(
                    new CanMoveToHereFactory<Sensor<Card>>(),
                    new TurnAfterReveal<Sensor<Card>>()
                )
            );
        }
    }
}
