namespace Snapdragon.Fluent.Selectors
{
    public class RevealedCardsForSide(bool OtherSide) : ISelector<ICard, IObjectWithSide>
    {
        public IEnumerable<ICard> Get(IObjectWithSide context, Game game)
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
