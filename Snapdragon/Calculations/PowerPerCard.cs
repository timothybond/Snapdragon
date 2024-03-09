namespace Snapdragon.Calculations
{
    public record PowerPerCard(ICardFilter<ICard> Filter, int PowerEach) : IPowerCalculation<ICard>
    {
        public int GetValue(Game game, ICard source, ICard target)
        {
            var count = game.AllCards.Where(c => Filter.Applies(c, source, game)).Count();

            return PowerEach * count;
        }
    }
}
