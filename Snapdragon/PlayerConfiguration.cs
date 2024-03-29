namespace Snapdragon
{
    public record PlayerConfiguration(
        string Name,
        Deck Deck,
        IPlayerController Controller,
        bool Shuffle = true
    )
    {
        public Player ToPlayer(Side side)
        {
            return new Player(this, side, 0);
        }
    }
}
