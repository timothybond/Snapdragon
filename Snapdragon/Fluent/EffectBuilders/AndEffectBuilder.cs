using Snapdragon.Effects;

namespace Snapdragon.Fluent.EffectBuilders
{
    public record AndEffectBuilder<TContext>(IEnumerable<IEffectBuilder<TContext>> EffectBuilders)
        : IEffectBuilder<TContext>
    {
        public AndEffectBuilder(params IEffectBuilder<TContext>[] effectBuilders)
            : this((IEnumerable<IEffectBuilder<TContext>>)effectBuilders) { }

        public IEffect Build(TContext context, Game game)
        {
            return new AndEffect(EffectBuilders.Select(eb => eb.Build(context, game)));
        }
    }
}
