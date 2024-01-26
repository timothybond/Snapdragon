namespace Snapdragon
{
    public static class Random
    {
        private static System.Random random = new System.Random();

        public static Side Side()
        {
            return random.Next(2) == 0 ? Snapdragon.Side.Top : Snapdragon.Side.Bottom;
        }

        public static Column Column()
        {
            switch (random.Next(3))
            {
                case 0:
                    return Snapdragon.Column.Left;
                case 1:
                    return Snapdragon.Column.Middle;
                case 2:
                    return Snapdragon.Column.Right;
                default:
                    throw new NotImplementedException();
            }
        }

        public static int Next(int maxValue = int.MaxValue)
        {
            return random.Next(maxValue);
        }

        public static bool NextBool()
        {
            return random.Next() % 2 == 0;
        }

        public static T Of<T>(IReadOnlyList<T> items)
        {
            var index = random.Next(items.Count);
            return items[index];
        }
    }
}
