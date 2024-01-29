namespace Snapdragon.CardAbilities
{
    public record AddCalculatedPowerSelf(Func<GameState, Card, int> PowerCalculation)
        : IRevealAbility<Card>
    {
        public GameState Activate(GameState game, Card source)
        {
            var powerToAdd = PowerCalculation(game, source);
            return game.WithModifiedCard(source, c => c with { Power = c.Power + powerToAdd });
        }
    }
}
