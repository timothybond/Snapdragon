namespace Snapdragon.Calculations
{
    public record PowerPerCard(ICardFilter<ICardInstance> Filter, int PowerEach) : IPowerCalculation<ICardInstance>
    {
        public int GetValue(Game game, ICardInstance source, ICardInstance target)
        {
            var count = game.AllCards.Where(c => Filter.Applies(c, source, game)).Count();

            return PowerEach * count;
        }
    }
}
