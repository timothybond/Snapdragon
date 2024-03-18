namespace Snapdragon.Fluent.Builders
{
    public record OngoingBuilder<TContext> : IBuilder<OnReveal<TContext>, TContext>
    {
        public OngoingBuilder() { }

        public RevealConditionBuilder<TContext> If
        {
            get { return new RevealConditionBuilder<TContext>(); }
        }

        public virtual OnReveal<TContext> Build(IEffectBuilder<TContext> effectBuilder)
        {
            return new OnReveal<TContext>(effectBuilder);
        }
    }
}
