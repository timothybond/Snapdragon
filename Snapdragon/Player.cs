using System.Collections.Immutable;

namespace Snapdragon
{
    public record Player(PlayerConfiguration Configuration, Side Side, int Energy, Library Deck, ImmutableList<Card> Hand)
    {
        public IPlayerController Controller => this.Configuration.Controller;
    }
}
