﻿namespace Snapdragon.Fluent.Builders
{
    public interface IBuilder<TAbility, TContext, TOutcome>
    {
        TAbility Build(TOutcome outcome);
    }

    public interface IBuilder<TAbility, TEvent, TContext, TOutcome>
    {
        TAbility Build(TOutcome outcome);
    }
}
