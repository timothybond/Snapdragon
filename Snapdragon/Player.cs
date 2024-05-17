using System.Collections.Immutable;

namespace Snapdragon
{
    public record Player(
        PlayerConfiguration Configuration,
        Side Side,
        int Energy,
        Library Library,
        ImmutableList<ICardInstance> Hand,
        ImmutableList<ICardInstance> Discards,
        ImmutableList<ICardInstance> Destroyed
    )
    {
        /// <summary>
        /// Constructor only suitable for the start of the game, where the rest of the parameters can be derived from
        /// those passed in.
        /// </summary>
        public Player(PlayerConfiguration configuration, Side side, bool shuffle = true)
            : this(configuration, side, 0, configuration.Deck.ToLibrary(side, shuffle), [], [], [])
        { }

        public IPlayerController Controller => this.Configuration.Controller;

        /// <summary>
        /// Draws a card from their library (if any remain).
        ///
        /// Note that this should generally NOT be invoked directly - use <see cref="Game.DrawCard(Side)"/>
        /// which will also log a <see cref="Events.CardDrawnEvent"/>.
        /// </summary>
        /// <returns></returns>
        public Player DrawCard()
        {
            if (Library.Count > 0 && Hand.Count < Max.HandSize)
            {
                var newHand = Hand.Add(
                    Library[0].ToCardInstance() with
                    {
                        State = CardState.InHand
                    }
                );
                var newLibrary = new Library(Library.Cards.RemoveAt(0));

                return this with
                {
                    Hand = newHand,
                    Library = newLibrary
                };
            }
            else
            {
                return this;
            }
        }

        public Player WithController(IPlayerController controller)
        {
            return this with
            {
                Configuration = this.Configuration with { Controller = controller }
            };
        }
    }
}
