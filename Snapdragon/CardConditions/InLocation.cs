namespace Snapdragon.CardConditions
{
    public record InLocation(Column Column) : ICardCondition
    {
        public bool IsMet(Game game, Card source)
        {
            return source.Column == this.Column;
        }
    }
}
