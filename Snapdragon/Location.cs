using System.Collections.Immutable;

namespace Snapdragon
{
    public record Location(string Name, Column Column, ImmutableList<Card> TopPlayerCards, ImmutableList<Card> BottomPlayerCards)
    {
        public Location(string Name, Column Column) : this(Name, Column, [], [])
        {
        }
    }
}
