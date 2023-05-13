using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MultilevelLibrary
{
    public static class MultilevelMazeIO
    {
        public static MultilevelMaze Load(string path)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.Open(path, FileMode.Open);
            MultilevelMaze maze = (MultilevelMaze)bf.Deserialize(fs);
            fs.Close();
            return maze;
        }

        public static void Save(MultilevelMaze maze, string path)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.Create(path);
            bf.Serialize(fs, maze);
            fs.Close();
        }

        public static MultilevelMaze LoadFromBytes(byte[] bytes)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(bytes);
            MultilevelMaze maze = (MultilevelMaze)bf.Deserialize(ms);
            ms.Close();
            return maze;
        }
    }
}
