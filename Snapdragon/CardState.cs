﻿namespace Snapdragon
{
    public enum CardState
    {
        /// <summary>
        /// The <see cref="ICardInstance"/> has not been drawn yet.
        /// </summary>
        InLibrary = 0,

        /// <summary>
        /// The <see cref="ICardInstance"/> has been drawn, but not played.
        /// </summary>
        InHand = 1,

        /// <summary>
        /// The <see cref="ICardInstance"/> has been played, but has not been revealed.
        /// </summary>
        PlayedButNotRevealed = 2,

        /// <summary>
        /// The <see cref="ICardInstance"/> has been played, and revealed, and remains in play.
        /// </summary>
        InPlay = 3,

        /// <summary>
        /// The <see cref="ICardInstance"/> has been discarded from the <see cref="Player"/>'s hand.  It's not immediately clear
        /// to me if this is distinct from "Destroyed" in a meaningful way, although the transitional event is handled
        /// differently by some cards.
        /// </summary>
        Discarded = 4,

        /// <summary>
        /// The <see cref="ICardInstance"/> has been destroyed (this can be from play, from the hand, or from the library).
        /// </summary>
        Destroyed = 5
    }
}
