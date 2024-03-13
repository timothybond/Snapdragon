using Snapdragon.Events;

namespace Snapdragon.TriggeredAbilities
{
    public record OnTurnEnd<T>(ISourceTriggeredEffectBuilder<T, TurnEndedEvent> EffectBuilder)
        : BaseTriggeredAbility<T, TurnEndedEvent>
    {
        protected override Game ProcessEvent(Game game, TurnEndedEvent e, T source)
        {
            var effect = EffectBuilder.Build(game, e, source);
            return effect.Apply(game);
        }
    }
}
