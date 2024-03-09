namespace Snapdragon.Calculations
{
    public record SpecificCardPower(ICard Card) : ICalculation
    {
        public int GetValue(Game game)
        {
            var actualCard = game.AllCards.SingleOrDefault(c => c.Id == Card.Id);

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
