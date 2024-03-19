namespace Snapdragon.Fluent
{
    public interface IEffectBuilder<in TContext> : IEffectBuilder<Event, TContext>
    {
        IEffect Build(TContext context, Game game);

        IEffect IEffectBuilder<Event, TContext>.Build(Event e, TContext context, Game game)
        {
            return this.Build(context, game);
        }
    }

    public interface IEffectBuilder<in TEvent, in TContext>
        where TEvent : Event
    {
        IEffect Build(TEvent e, TContext context, Game game);
    }
}
