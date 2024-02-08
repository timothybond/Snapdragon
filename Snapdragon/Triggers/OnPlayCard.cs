using Snapdragon.Events;

namespace Snapdragon.Triggers
{
    /// <summary>
    /// A trigger that fires when a card is played (not revealed).
    ///
    /// If specified, the attributes narrow down what card plays actually
    /// fire the trigger.
    /// </summary>
    /// <param name="Column"></param>
    /// <param name="Side"></param>
    /// <param name="Turn"></param>
    public record OnPlayCard(Column? Column, Side? Side, int? Turn) : ITrigger<CardPlayedEvent>
    {
        public bool IsMet(CardPlayedEvent e, Game game)
        {
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
