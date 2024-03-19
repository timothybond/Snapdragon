namespace Snapdragon.Fluent.Builders
{
    public record OngoingBuilder<TContext>
        : IBuilder<Ongoing<TContext>, TContext, IOngoingAbilityFactory<TContext>>
    {
        public OngoingBuilder() { }

        public ConditionBuilder<Ongoing<TContext>, TContext, IOngoingAbilityFactory<TContext>> If
        {
            get
            {
                return new ConditionBuilder<
                    Ongoing<TContext>,
                    TContext,
                    IOngoingAbilityFactory<TContext>
                >(new OngoingFactory());
            }
        }

        public virtual Ongoing<TContext> Build(IOngoingAbilityFactory<TContext> outcome)
        {
            return outcome.Build();
        }

        private class OngoingFactory
            : IResultFactory<Ongoing<TContext>, TContext, IOngoingAbilityFactory<TContext>>
        {
            public Ongoing<TContext> Build(
                IOngoingAbilityFactory<TContext> outcome,
                ICondition<TContext>? condition = null
            )
            {
                return outcome.Build(condition);
            }
        }
    }
}
