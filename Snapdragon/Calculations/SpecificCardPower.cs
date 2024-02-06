namespace Snapdragon.Calculations
{
    public record SpecificCardPower(Card card) : ICalculation
    {
        public int GetValue(Game game)
        {
            var actualCard = game.AllCards.SingleOrDefault(c => c.Id == card.Id);

            if (actualCard == null)
            {
                throw new InvalidOperationException(
                    "Specified card (for power calculation) not found in play."
                );
            }

            return actualCard.Power;
        }
    }
}
