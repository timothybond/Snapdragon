namespace Snapdragon
{
    public interface ITriggeredAbility<TSource>
    {
        Game ProcessEvent(Game game, Event e, TSource source);

        bool DiscardedOrDestroyed()
        {
            return this is ISpecialCardTrigger cardTrigger && cardTrigger.WhenDiscardedOrDestroyed;
        }

        bool InHand()
        {
            return this is ISpecialCardTrigger cardTrigger && cardTrigger.WhenInHand;
        }

        bool InDeck()
        {
            return this is ISpecialCardTrigger cardTrigger && cardTrigger.WhenInDeck;
        }
    }
}
