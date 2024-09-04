using System.IO;

namespace OrzAutoEntity.Helpers
{
    public static class ConfigHelper
    {
        /// <summary>
        /// 配置文件是否存在
        /// </summary>
        /// <returns></returns>
        public static bool HasConfigFile()
        {
            return File.Exists(GetConfigFullPath());
        }

        private static string GetConfigFullPath()
        {
            var path = DTEHelper.GetSelectedProjectFullPath();
            return Path.Combine(path, "__entity.xml");
        }
    }
}
