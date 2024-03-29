namespace Snapdragon
{
    public static class Ids
    {
        private const string Card = "Card";
        private const string Sensor = "Sensor";

        private static Dictionary<string, long> currentMaxIds = new Dictionary<string, long>
        {
            { Card, 0 },
            { Sensor, 0 }
        };

        public static long GetNextCard()
        {
            return GetNext(Card);
        }

        public static long GetNextSensor()
        {
            return GetNext(Sensor);
        }

        private static long GetNext(string name)
        {
            lock (currentMaxIds)
            {
                currentMaxIds[name] = currentMaxIds[name] + 1;

                return currentMaxIds[name];
            }
        }
    }
}
