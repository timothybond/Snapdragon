namespace Snapdragon.RevealAbilities
{
    public record OnRevealIf(ICardCondition Condition, IRevealAbility<Card> TriggeredAbility) : IRevealAbility<Card>
    {
        public GameState Activate(GameState game, Card source)
        {
            if (Condition.IsMet(game, source))
            {
                return TriggeredAbility.Activate(game, source);
            }

            return game;
        }
    }
}
