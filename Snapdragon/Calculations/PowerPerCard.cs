namespace Snapdragon.Calculations
{
    public record PowerPerCard(ICardFilter<Card> Filter, int PowerEach) : IPowerCalculation<Card>
    {
        public int GetValue(Game game, Card source, Card target)
        {
            var count = game.AllCards.Where(c => Filter.Applies(c, source, game)).Count();

            return PowerEach * count;
        }
    }
}
