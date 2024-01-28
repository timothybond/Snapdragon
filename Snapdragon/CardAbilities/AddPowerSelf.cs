namespace Snapdragon.CardAbilities
{
    public record AddPowerSelf(int Power) : ICardRevealAbility
    {
        public GameState Activate(GameState game, Card source)
        {
            return game.WithModifiedCard(source, c => c with { Power = c.Power + this.Power });
        }
    }
}
