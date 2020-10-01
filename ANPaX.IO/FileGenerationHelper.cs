using System.IO;

namespace ANPaX.IO
{
    internal static class FileGenerationHelper
    {

        public static void GenerateFolder(string file)
        {
            var path = Path.GetDirectoryName(file);
            Directory.CreateDirectory(path);
        }

    }
}
