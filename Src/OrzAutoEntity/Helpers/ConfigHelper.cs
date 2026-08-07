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
        public static List<ColumnConfig> Columns { get; private set; }

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
            Columns = ColumnConfig.Reload(doc);
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

        public static List<TemplateConfig> GetTemplateConfig(string[] templateIds)
        {
            return Templates.Where(t => templateIds.Contains(t.Id)).ToList();
        }

        public static List<FilterConfig> GetFilterConfig(string[] filterIds, FilterType type)
        {
            return Filters.Where(t => t.Type == type && filterIds.Contains(t.Id)).ToList();
        }

        public static IEnumerable<ColumnConfig> GetColumnConfig(string[] columnIds)
        {
            return Columns.Where(t => columnIds.Contains(t.Id));
        }

        private static string GetConfigFullPath(string configPath)
        {
            return Path.Combine(configPath, ConfigFileName);
        }
    }
}
