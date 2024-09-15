using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using OrzAutoEntity.Modes;

namespace OrzAutoEntity.Helpers
{
    public static class ConfigHelper
    {
        public const string ConfigFileName = "__entity.xml";
        public static List<TemplateConfig> Templates { get; private set; }
        public static List<DatabaseConfig> Databases { get; private set; }
        public static List<FilterConfig> Filters { get; private set; }

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
            Filters = FilterConfig.Reload(doc);
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

        public static DatabaseConfig GetDatabaseConfig(string dbType)
        {
            return Databases.FirstOrDefault(t => t.Type == dbType);
        }

        public static TemplateConfig GetTemplateConfig(string templateId)
        {
            return Templates.FirstOrDefault(t => t.Id == templateId);
        }

        public static FilterConfig GetFilterConfig(string filterId)
        {
            return Filters.FirstOrDefault(t => t.Id == filterId) ?? FilterConfig.Default;
        }

        private static string GetConfigFullPath(string configPath)
        {
            return Path.Combine(configPath, ConfigFileName);
        }
    }
}
