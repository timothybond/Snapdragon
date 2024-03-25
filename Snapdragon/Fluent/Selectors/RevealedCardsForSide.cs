namespace Snapdragon.Fluent.Selectors
{
    public class RevealedCardsForSide(bool OtherSide) : ISelector<Card, IObjectWithSide>
    {
        public IEnumerable<Card> Get(IObjectWithSide context, Game game)
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
