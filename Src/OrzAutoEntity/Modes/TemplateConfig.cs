using System.Collections.Generic;
using System.Xml;

namespace OrzAutoEntity.Modes
{
    public class TemplateConfig
    {
        /// <summary>
        /// 模板id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 模板内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<TemplateConfig> Reload(XmlDocument doc)
        {
            var result = new List<TemplateConfig>();
            var nodes = doc.SelectNodes("AutoEntity/Templates/Template");
            foreach (XmlElement node in nodes)
            {
                result.Add(new TemplateConfig
                {
                    Id = node.GetAttribute("id"),
                    Content = node.InnerText.Trim(),
                });
            }
            return result;
        }
    }
}
