using Snapdragon.Events;

namespace Snapdragon.TriggeredAbilities
{
    public record OnSpecificTurnEnd<T>(
        int Turn,
        ISourceTriggeredEffectBuilder<T, TurnEndedEvent> EffectBuilder
    ) : BaseTriggeredAbility<T, TurnEndedEvent>
    {
        protected override Game ProcessEvent(Game game, TurnEndedEvent e, T source)
        {
            if (e.Turn != Turn)
            {
                return game;
            }

            var effect = EffectBuilder.Build(game, e, source);
            return effect.Apply(game);
        }
    }
}
