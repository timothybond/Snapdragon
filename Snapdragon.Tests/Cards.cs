using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapdragon.Tests
{
    public class Cards
    {
        public static readonly CardDefinition OneOne = new CardDefinition("OneOne", 1, 1);
        public static readonly CardDefinition OneTwo = new CardDefinition("OneTwo", 1, 2);
        public static readonly CardDefinition OneThree = new CardDefinition("OneThree", 1, 3);

        public static readonly CardDefinition TwoOne = new CardDefinition("TwoOne", 2, 1);
        public static readonly CardDefinition TwoTwo = new CardDefinition("TwoTwo", 2, 2);
        public static readonly CardDefinition TwoThree = new CardDefinition("TwoThree", 2, 3);
    }
}
