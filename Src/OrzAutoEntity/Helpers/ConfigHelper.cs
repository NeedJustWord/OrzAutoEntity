using System.Collections.Generic;
using System.IO;
using System.Xml;
using OrzAutoEntity.Modes;

namespace OrzAutoEntity.Helpers
{
    public static class ConfigHelper
    {
        public static List<TemplateConfig> Templates { get; private set; }
        public static List<DatabaseConfig> Databases { get; private set; }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="configPath"></param>
        public static void Init(string configPath)
        {
            var doc = new XmlDocument();
            doc.Load(GetConfigFullPath(configPath));
            TypeMapping.Reload(doc);
            Templates = TemplateConfig.Reload(doc);
            Databases = DatabaseConfig.Reload(doc);
        }

        /// <summary>
        /// 配置文件是否存在
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static bool HasConfigFile(string configPath)
        {
            return File.Exists(GetConfigFullPath(configPath));
        }

        private static string GetConfigFullPath(string configPath)
        {
            return Path.Combine(configPath, "__entity.xml");
        }
    }
}
