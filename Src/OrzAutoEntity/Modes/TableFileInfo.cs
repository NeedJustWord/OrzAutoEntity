using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OrzAutoEntity.Modes
{
    /// <summary>
    /// 表生成文件信息
    /// </summary>
    public class TableFileInfo
    {
        #region 实体类属性
        public string FilePath { get; set; }

        public TemplateConfig Config { get; set; }

        public TableInfo Table { get; set; }
        #endregion

        #region 静态
        private static PropertyInfo[] tableInfoPropertyInfos;

        static TableFileInfo()
        {
            tableInfoPropertyInfos = typeof(TableInfo).GetProperties().OrderByDescending(t => t.Name.Length).ToArray();
        }

        /// <summary>
        /// 获取表生成文件信息列表
        /// </summary>
        /// <param name="templates"></param>
        /// <param name="tables"></param>
        /// <returns></returns>
        public static List<TableFileInfo> GetTableFileInfos(List<TemplateConfig> templates, List<TableInfo> tables)
        {
            var dict = new Dictionary<string, TableFileInfo>(templates.Count * tables.Count, StringComparer.OrdinalIgnoreCase);
            foreach (var config in templates)
            {
                foreach (var table in tables)
                {
                    var info = GetTableFileInfo(config, table);
                    dict[info.FilePath] = info;
                }
            }
            return dict.Values.ToList();
        }

        private static TableFileInfo GetTableFileInfo(TemplateConfig config, TableInfo table)
        {
            var filePath = GetFilePath(config, table);
            return new TableFileInfo
            {
                FilePath = filePath,
                Config = config,
                Table = table,
            };
        }

        private static string GetFilePath(TemplateConfig config, TableInfo table)
        {
            var filePath = config.FilePath.IsNotNullAndEmpty()
                ? Path.Combine(table.FilePath, config.FilePath)
                : Path.Combine(table.FilePath, $"{table.LowerName}.cs");
            foreach (var property in tableInfoPropertyInfos)
            {
                var key1 = $"$Table.{property.Name}";
                var key2 = $"${{Table.{property.Name}}}";
                var hasKey1 = filePath.Contains(key1);
                var hasKey2 = filePath.Contains(key2);

                if (hasKey1 || hasKey2)
                {
                    var propertyValue = property.GetValue(table);
                    var newValue = propertyValue.ToString();
                    if (hasKey1)
                    {
                        filePath = filePath.Replace(key1, newValue);
                    }
                    if (hasKey2)
                    {
                        filePath = filePath.Replace(key2, newValue);
                    }
                }
            }
            return filePath.PathFormat();
        }
        #endregion
    }
}
