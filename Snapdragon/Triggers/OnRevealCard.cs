using Snapdragon.Events;

namespace Snapdragon.Triggers
{
    /// <summary>
    /// A trigger that fires when a card is revealed.
    ///
    /// If specified, the attributes narrow down what card plays actually
    /// fire the trigger.
    /// </summary>
    /// <param name="Column"></param>
    /// <param name="Side"></param>
    /// <param name="Turn"></param>
    /// <param name="Ignored">If specified, this card is ignored.
    /// Used for scenarios where we don't want to trigger a card's
    /// ability by revealing itself (which is most cases).</param>
    public record OnRevealCard(Column? Column, Side? Side, int? Turn, Card? Ignored)
        : ITrigger<CardRevealedEvent>
    {
        public bool IsMet(CardRevealedEvent e, Game game)
        {
            if (e.Card.Id == Ignored?.Id)
            {
                return false;
            }

            if (this.Column.HasValue && this.Column.Value != e.Card.Column)
            {
                return false;
            }

            if (this.Side.HasValue && this.Side.Value != e.Card.Side)
            {
                return false;
            }

            if (this.Turn.HasValue && this.Turn.Value != game.Turn)
            {
                return false;
            }

            return true;
        }
    }
}
