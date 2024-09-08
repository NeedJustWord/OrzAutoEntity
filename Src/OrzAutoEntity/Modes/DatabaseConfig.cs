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
        /// 对应模板id
        /// </summary>
        public string TemplateId { get; set; }
        /// <summary>
        /// 生成目录
        /// </summary>
        public string Directory { get; set; }
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
                    TemplateId = node.GetAttribute("templateId"),
                    Directory = node.GetAttribute("directory"),
                    ConnString = node.GetAttribute("connString"),
                });
            }
            return result;
        }
    }
}
