using Snapdragon.Fluent.Builders;
using Snapdragon.Fluent.EffectBuilders;
using Snapdragon.Fluent.EventFilters;

namespace Snapdragon.Fluent
{
    public static class NextTurn
    {
        public static OnReveal<ICard> CanMoveHere()
        {
            return new CardRevealed().Build(
                new CreateMoveSensorBuilder(
                    new CanMoveToHereFactory<Sensor<ICard>>(),
                    new TurnAfterReveal<Sensor<ICard>>()
                )
            );
        }
    }
}
