namespace Snapdragon.Effects
{
    public record DestroyCardInPlay(Card Card) : IEffect
    {
        public Game Apply(Game game)
        {
            throw new NotImplementedException();
        }
    }
}
