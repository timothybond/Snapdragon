namespace Snapdragon
{
    public interface ICardCondition
    {
        /// <summary>
        /// Checks if the condition is satisified.
        /// </summary>
        /// <param name="game">Current game state.</param>
        /// <param name="source">
        /// The <see cref="Card"/> on which the condition was defined.
        ///
        /// Conditions are often in reference to this card (e.g., things happening in the same location).
        /// </param>
        bool IsMet(GameState game, Card source);
    }
}
