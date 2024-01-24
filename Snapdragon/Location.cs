using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapdragon
{
    public record Location(ImmutableList<Card> TopPlayerCards, ImmutableList<Card> BottomPlayerCards)
    {
    }
}
