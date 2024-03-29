namespace Snapdragon.Effects
{
    public record DestroyCardInPlay(ICardInstance Card) : IEffect
    {
        public Game Apply(Game game)
        {
            var card = game.AllCards.SingleOrDefault(c => c.Id == Card.Id);

            if (card == null)
            {
                return game;
            }

            if (game.GetBlockedEffects(card).Contains(EffectType.DestroyCard))
            {
                return game;
            }

            return game.DestroyCardInPlay(card);
        }
    }
}
