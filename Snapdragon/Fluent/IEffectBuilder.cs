namespace Snapdragon.Fluent
{
    public interface IEffectBuilder<in TContext> : IEffectBuilder<Event, TContext>
    {
        IEffect Build(TContext context, Game game);

        IEffect IEffectBuilder<Event, TContext>.Build(Event e, TContext context, Game game)
        {
            return this.Build(context, game);
        }

        IEffectBuilder<TEvent, TContext> ForEvent<TEvent>()
            where TEvent : Event
        {
            return new WrappedEffectBuilder<TEvent, TContext>(this);
        }
    }

    public interface IEffectBuilder<in TEvent, in TContext>
        where TEvent : Event
    {
        IEffect Build(TEvent e, TContext context, Game game);
    }

    public record WrappedEffectBuilder<TEvent, TContext>(IEffectBuilder<TContext> InnerBuilder)
        : IEffectBuilder<TEvent, TContext>
        where TEvent : Event
    {
        public IEffect Build(TEvent e, TContext context, Game game)
        {
            return InnerBuilder.Build(context, game);
        }
    }
}
