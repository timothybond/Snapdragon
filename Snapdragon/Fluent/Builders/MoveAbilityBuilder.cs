namespace Snapdragon.Fluent.Builders
{
    public record MoveAbilityBuilder<TContext>
        : IBuilder<IMoveAbility<TContext>, TContext, IMoveAbilityFactory<TContext>>
    {
        public ConditionBuilder<IMoveAbility<TContext>, TContext, IMoveAbilityFactory<TContext>> If
        {
            get
            {
                return new ConditionBuilder<
                    IMoveAbility<TContext>,
                    TContext,
                    IMoveAbilityFactory<TContext>
                >(new MoveAbilityFactory());
            }
        }

        public virtual IMoveAbility<TContext> Build(IMoveAbilityFactory<TContext> outcome)
        {
            return outcome.Build();
        }

        private class MoveAbilityFactory
            : IResultFactory<IMoveAbility<TContext>, TContext, IMoveAbilityFactory<TContext>>
        {
            public IMoveAbility<TContext> Build(
                IMoveAbilityFactory<TContext> outcome,
                ICondition<TContext>? condition = null
            )
            {
                return outcome.Build();
            }
        }
    }
}
