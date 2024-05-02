namespace Snapdragon.Fluent.Selectors
{
    public class RevealedCardsForSide(bool OtherSide) : ISelector<ICardInstance, IObjectWithSide>
    {
        public IEnumerable<ICardInstance> Get(IObjectWithSide context, Game game)
        {
            var side = context.Side;
            if (OtherSide)
            {
                side = side.Other();
            }

            return game.AllCards.Where(c => c.Side == side);
        }

        public bool Selects(ICardInstance item, IObjectWithSide context, Game game)
        {
            return item.State == CardState.InPlay && item.Side == context.Side;
        }
    }
}
