namespace Snapdragon.Fluent.CardSelectors
{
    public interface ICardFilter<in TContext>
    {
        bool Includes(ICard card, TContext context);
    }
}
