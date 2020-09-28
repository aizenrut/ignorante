using System.Text;

namespace Ignorante.Pools
{
    public static class StringBuilderPool
    {
        private static int currentPoolIndex = 0;
        private static StringBuilder[] pool = { new StringBuilder(), new StringBuilder(), new StringBuilder(), new StringBuilder(), new StringBuilder() };

        private static void IncrementarIndicePool()
        {
            if (currentPoolIndex == pool.Length - 1)
            {
                currentPoolIndex = 0;
            }

            currentPoolIndex++;
        }

        public static StringBuilder ObterDaPool()
        {
            var current = pool[currentPoolIndex];

            current.Clear();

            IncrementarIndicePool();

            return current;
        }
    }
}
