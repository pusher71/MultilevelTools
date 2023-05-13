using System.Collections.Generic;

namespace MultilevelLibrary
{
    public class Shuffler<T>
    {
        public static void ShuffleList(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T key = list[i];
                int rnd = r.Next(i, list.Count);
                list[i] = list[rnd];
                list[rnd] = key;
            }
        }
    }
}
