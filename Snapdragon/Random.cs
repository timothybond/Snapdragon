using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snapdragon
{
    static class Random
    {
        private static System.Random random = new System.Random();
        
        public static Side Side()
        {
            return random.Next(2) == 0
                ? Snapdragon.Side.Top
                : Snapdragon.Side.Bottom;
        }

        public static int Next(int maxValue = int.MaxValue)
        {
            return random.Next(maxValue);
        }
    }
}
