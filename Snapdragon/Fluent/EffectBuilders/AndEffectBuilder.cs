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

    public record AndEffectBuilder<TEvent, TContext>(
        IEnumerable<IEffectBuilder<TEvent, TContext>> EffectBuilders
    ) : IEffectBuilder<TEvent, TContext>
        where TEvent : Event
    {
        public AndEffectBuilder(params IEffectBuilder<TEvent, TContext>[] effectBuilders)
            : this((IEnumerable<IEffectBuilder<TEvent, TContext>>)effectBuilders) { }

        public IEffect Build(TEvent e, TContext context, Game game)
        {
            return new AndEffect(EffectBuilders.Select(eb => eb.Build(e, context, game)));
        }
    }
}
