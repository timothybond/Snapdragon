namespace Snapdragon.TargetFilters
{
    public static class TargetFiltersExtensions
    {
        public static ICardFilter<T> And<T>(this ICardFilter<T> first, ICardFilter<T> second)
        {
            return new AndFilter<T>(first, second);
        }
    }
}
