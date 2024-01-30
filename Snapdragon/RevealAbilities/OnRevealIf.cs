namespace Snapdragon.RevealAbilities
{
    public record OnRevealIf(ICardCondition Condition, IRevealAbility<Card> TriggeredAbility) : IRevealAbility<Card>
    {
        public Game Activate(Game game, Card source)
        {
            if (Condition.IsMet(game, source))
            {
                return TriggeredAbility.Activate(game, source);
            }

            return game;
        }
    }
}
