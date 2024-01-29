namespace Snapdragon.Effects
{
    /// <summary>
    /// An <see cref="IEffect"/> that adds a calculated amount of power to the specified <see cref="Card"/>s.
    ///
    /// Note that this is permanent power, not as an ongoing effect.
    /// </summary>
    public record AddPowerTo(ICardFilter Targets, ICalculation Amount) : IEffect
    {
        public GameState Apply(GameState game)
        {
            var power = Amount.GetValue(game);
            var cards = game
                .AllCards.Where(Targets.Applies)
                .Select(c => c with { Power = c.Power + power });

            return game.WithCards(cards);
        }
    }
}
