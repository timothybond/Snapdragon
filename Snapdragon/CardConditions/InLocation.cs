namespace Snapdragon.CardConditions
{
    public record InLocation(Column Column) : ICardCondition
    {
        public bool IsMet(GameState game, Card source)
        {
            return source.Column == this.Column;
        }
    }
}
