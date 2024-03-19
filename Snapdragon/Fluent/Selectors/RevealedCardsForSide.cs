namespace Snapdragon.Fluent.Selectors
{
    public class RevealedCardsForSide(bool OtherSide) : ICardSelector<ICard>
    {
        public IEnumerable<ICard> Get(ICard context, Game game)
        {
            var side = context.Side;
            if (OtherSide)
            {
                side = side.Other();
            }

            return game.AllCards.Where(c => c.Side == side);
        }
    }
}
