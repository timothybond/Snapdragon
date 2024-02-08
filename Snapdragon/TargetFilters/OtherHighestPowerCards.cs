namespace Snapdragon.TargetFilters
{
    /// <summary>
    /// A filter that gets all of the cards on the same side that are tied for the highest power,
    /// ignoring the triggering card. (I.e., for Doctor Strange's power.)
    /// </summary>
    public class OtherHighestPowerCards : ICardFilter<Card>
    {
        public bool Applies(Card card, Card source, Game game)
        {
            // These are sort of redundant but will speed it up (not that that likely matters).
            if (card.Side != source.Side)
            {
                return false;
            }

            if (card.Id == source.Id)
            {
                return false;
            }

            var sameSideExcludingSelf = game.AllCards.Where(c =>
                c.Side == source.Side && c.Id != source.Id
            );

            // TODO: Determine if we should use AdjustedPower instead
            var maxPower = sameSideExcludingSelf.Select(c => c.Power).Max();

            return card.Power == maxPower;
        }
    }
}
