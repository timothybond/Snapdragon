namespace Snapdragon.TargetFilters
{
    public static class TargetFiltersExtensions
    {
        public static ICardFilter And(this ICardFilter first, ICardFilter second)
        {
            return new AndFilter(first, second);
        }
    }
}
