using System.Collections.Generic;
using System.Xml;

namespace OrzAutoEntity.Modes
{
    public class FilterConfig
    {
        public static readonly FilterConfig Default = new FilterConfig();

        public string Id { get; set; }
        public List<string> EqualsFilter { get; set; }
        public List<string> StartsWithFilter { get; set; }
        public List<string> EndsWithFilter { get; set; }
        public List<string> ContainsFilter { get; set; }
        public int Count => EqualsFilter?.Count + StartsWithFilter?.Count + EndsWithFilter?.Count + ContainsFilter?.Count ?? 0;

        public FilterConfig(bool init = false)
        {
            if (init)
            {
                EqualsFilter = new List<string>();
                StartsWithFilter = new List<string>();
                EndsWithFilter = new List<string>();
                ContainsFilter = new List<string>();
            }
        }

        public FilterConfig(string id) : this(true)
        {
            Id = id;
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<FilterConfig> Reload(XmlDocument doc)
        {
            var result = new List<FilterConfig>();
            var nodes = doc.SelectNodes("AutoEntity/Filters/Filter");
            foreach (XmlElement node in nodes)
            {
                var config = new FilterConfig(node.GetAttribute("id"));
                foreach (XmlNode item in node.SelectNodes("Item"))
                {
                    var filter = item.InnerText.Trim();
                    if (filter.StartsWith("%"))
                    {
                        if (filter.EndsWith("%"))
                        {
                            config.ContainsFilter.AddRange(filter.Trim('%').Split('|'));
                        }
                        else
                        {
                            config.EndsWithFilter.AddRange(filter.TrimStart('%').Split('|'));
                        }
                    }
                    else if (filter.EndsWith("%"))
                    {
                        config.StartsWithFilter.AddRange(filter.TrimEnd('%').Split('|'));
                    }
                    else
                    {
                        config.EqualsFilter.AddRange(filter.Split('|'));
                    }
                }
                result.Add(config);
            }
            return result;
        }
    }
}
