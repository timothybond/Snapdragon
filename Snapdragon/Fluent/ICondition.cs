namespace Snapdragon.Fluent
{
    public interface ICondition<TContext>
    {
        bool IsMet(TContext context, Game game);
    }
}
