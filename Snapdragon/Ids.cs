namespace Snapdragon
{
    public static class Ids
    {
        private static Dictionary<string, long> currentMaxIds = new Dictionary<string, long>();

        public static long GetNext<T>()
        {
            var name = typeof(T).AssemblyQualifiedName;

            if (name == null)
            {
                throw new ArgumentNullException($"Could not get type name for type: {typeof(T)}");
            }

            lock (currentMaxIds)
            {
                if (!currentMaxIds.ContainsKey(name))
                {
                    currentMaxIds[name] = 0;
                }

                currentMaxIds[name] = currentMaxIds[name] + 1;

                return currentMaxIds[name];
            }
        }
    }
}
