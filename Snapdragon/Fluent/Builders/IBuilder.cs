namespace Snapdragon.Fluent.Builders
{
    public interface IBuilder<TAbility, TContext, TOutcome>
    {
        TAbility Build(TOutcome outcome);
    }
}
