using System;

namespace MultilevelLibrary
{
    static class r
    {
        private static Random rand;
        public static void Init(int seed) => rand = new Random(seed);
        public static int Next(int max) => rand.Next(max);
        public static int Next(int min, int max) => rand.Next(min, max);
    }
}
