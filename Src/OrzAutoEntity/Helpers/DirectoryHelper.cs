using System.IO;

namespace OrzAutoEntity.Helpers
{
    public static class DirectoryHelper
    {
        public static void CreateDirectory(string path)
        {
            CreateDirectory(new DirectoryInfo(path));
        }

        public static void CreateDirectory(DirectoryInfo info)
        {
            if (info.Exists) return;
            CreateDirectory(info.Parent);
            info.Create();
        }
    }
}
