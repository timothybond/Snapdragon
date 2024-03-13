using Snapdragon.Effects;

namespace Snapdragon.TriggeredEffects
{
    public record DestroyCardsHere : ISourceTriggeredEffectBuilder<IObjectWithColumn, Event>
    {
        public IEffect Build(Game game, Event e, IObjectWithColumn source)
        {
            var destroyEffects = game[source.Column].AllCards.Select(c => new DestroyCardInPlay(c));

            var aggregateEffect = destroyEffects.Aggregate(
                (IEffect)new NullEffect(),
                (acc, e) => new AndEffect(acc, e)
            );

            return aggregateEffect;
        }
    }
}
