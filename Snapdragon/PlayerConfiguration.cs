﻿namespace Snapdragon
{
    public record PlayerConfiguration(string Name, Deck Deck, IPlayerController Controller)
    {
        public Player ToPlayer(Side side, bool shuffle = true)
        {
            return new Player(this, side, 0, Deck.ToLibrary(side, shuffle), [], [], []);
        }
    }
}
