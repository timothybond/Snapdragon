namespace Snapdragon.Fluent.Conditions
{
    public record AfterTurnCondition(int Turn) : ICondition<object>
    {
        public bool IsMet(object context, Game game)
        {
            return game.Turn > Turn;
        }
    }
}
