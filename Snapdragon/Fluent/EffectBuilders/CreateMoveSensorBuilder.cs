using Snapdragon.Events;
using Snapdragon.Fluent.Builders;

namespace Snapdragon.Fluent.EffectBuilders
{
    public record CreateMoveSensorBuilder(
        IMoveAbilityFactory<Sensor<Card>> MoveAbilityFactory,
        ICondition<Sensor<Card>> Condition,
        int? expiringAfterTurns = null
    ) : IEffectBuilder<Card>
    {
        public IEffect Build(Card context, Game game)
        {
            if (expiringAfterTurns != null)
            {
                var deleteSelfAfterTurns = When
                    .Sensor.InPlayAnd<TurnEndedEvent>()
                    .If.AfterTurn(game.Turn + expiringAfterTurns.Value - 1) // This reference is a bit janky but I didn't feel like building a "greater than or equal to turns" at the moment
                    .Build(new DestroySensorBuilder());

                return new Snapdragon.Effects.CreateMoveSensor(
                    context,
                    MoveAbilityFactory.Build(Condition),
                    deleteSelfAfterTurns
                );
            }
            else
            {
                return new Snapdragon.Effects.CreateMoveSensor(context, MoveAbilityFactory.Build(Condition));
            }
        }
    }
}
