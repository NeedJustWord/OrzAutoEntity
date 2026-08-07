using System;
using System.Collections.Generic;
using System.Xml;

namespace OrzAutoEntity.Modes
{
    public class DatabaseConfig
    {
        /// <summary>
        /// 数据库名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 对应模板id列表
        /// </summary>
        public string[] TemplateIds { get; set; }
        /// <summary>
        /// 对应过滤器id列表
        /// </summary>
        public string[] FilterIds { get; set; }
        /// <summary>
        /// 对应字段配置id列表
        /// </summary>
        public string[] ColumnIds { get; set; }
        /// <summary>
        /// 生成目录
        /// </summary>
        public string Directory { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected { get; set; }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnString { get; set; }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<DatabaseConfig> Reload(XmlDocument doc)
        {
            var result = new List<DatabaseConfig>();
            var nodes = doc.SelectNodes("AutoEntity/DataSource/Database");
            foreach (XmlElement node in nodes)
            {
                result.Add(new DatabaseConfig
                {
                    Name = node.GetAttribute("name"),
                    Type = node.GetAttribute("type"),
                    TemplateIds = node.GetAttribute("templateId").SplitRemoveEmptyEntries(','),
                    FilterIds = node.GetAttribute("filterId").SplitRemoveEmptyEntries(','),
                    ColumnIds = node.GetAttribute("columnId").SplitRemoveEmptyEntries(','),
                    Directory = node.GetAttribute("directory"),
                    IsSelected = node.GetAttribute("isSelected").AsBool(),
                    ConnString = node.GetAttribute("connString"),
                });
            }
            return result;
        }
    }
}
