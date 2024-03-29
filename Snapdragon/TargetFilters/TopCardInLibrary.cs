namespace Snapdragon.TargetFilters
{
    public record TopCardInLibrary<T> : ICardFilter<T>
    {
        public bool Applies(ICardInstance card, T source, Game game)
        {
            var library = game[card.Side].Library;

            return library.Count > 0 && library[0].Id == card.Id;
        }
    }
}
