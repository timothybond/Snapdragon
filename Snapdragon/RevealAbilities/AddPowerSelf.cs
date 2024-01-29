namespace Snapdragon.RevealAbilities
{
    public record AddPowerSelf(int Power) : IRevealAbility<Card>
    {
        public GameState Activate(GameState game, Card source)
        {
            return game.WithModifiedCard(source, c => c with { Power = c.Power + Power });
        }
    }
}
