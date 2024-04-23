namespace Snapdragon.Fluent.Builders
{
    public interface IBuilder<TAbility, TContext, TOutcome>
    {
        TAbility Then(TOutcome outcome);
    }

    public interface IBuilder<TAbility, TEvent, TContext, TOutcome>
    {
        TAbility Build(TOutcome outcome);
    }
}
