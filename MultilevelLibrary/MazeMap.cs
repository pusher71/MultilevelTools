using System;
using PrimitiveData3D;

namespace MultilevelLibrary
{
    [Serializable]
    public class MazeMap<T>
    {
        public int Width { get; }
        public int Height { get; }
        public int Count { get; }

        private readonly T[,,] array;

        public MazeMap(int width, int height, int count)
        {
            Width = width;
            Height = height;
            Count = count;
            array = new T[width, height, count];
        }

        public MazeMap<T> Clone()
        {
            MazeMap<T> copy = new MazeMap<T>(Width, Height, Count);
            for (int z = 0; z < Count; z++)
                for (int y = 0; y < Height; y++)
                    for (int x = 0; x < Width; x++)
                        copy.array[x, y, z] = array[x, y, z];
            return copy;
        }

        public T Get(Vector3P position) => array[position.X, position.Y, position.Z];
        public void Set(Vector3P position, T item) => array[position.X, position.Y, position.Z] = item;
        public T Get(int x, int y, int z) => array[x, y, z];
        public void Set(int x, int y, int z, T item) => array[x, y, z] = item;
    }
}
