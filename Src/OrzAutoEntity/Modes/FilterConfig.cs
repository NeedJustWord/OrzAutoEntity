using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace OrzAutoEntity.Modes
{
    public class FilterConfig
    {
        public static readonly FilterConfig Default = new FilterConfig(string.Empty);

        public string Id { get; set; }

        private Dictionary<Operate, Item> dict;
        private delegate bool NameHandle(string name, string key, out string entityName);

        public FilterConfig(string id)
        {
            Id = id;
            dict = new Dictionary<Operate, Item>();
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
                foreach (XmlElement element in node.SelectNodes("Item"))
                {
                    var filter = element.InnerText.Trim();
                    var item = GetItem(element.GetAttribute("operate"), config);
                    if (filter.StartsWith("%"))
                    {
                        if (filter.EndsWith("%"))
                        {
                            item.ContainsFilter.AddRange(filter.Trim('%').SplitRemoveEmptyEntries('|'));
                        }
                        else
                        {
                            item.EndsWithFilter.AddRange(filter.TrimStart('%').SplitRemoveEmptyEntries('|'));
                        }
                    }
                    else if (filter.EndsWith("%"))
                    {
                        item.StartsWithFilter.AddRange(filter.TrimEnd('%').SplitRemoveEmptyEntries('|'));
                    }
                    else
                    {
                        item.EqualsFilter.AddRange(filter.SplitRemoveEmptyEntries('|'));
                    }
                }
                result.Add(config);
            }
            return result;
        }

        private static Item GetItem(string operateStr, FilterConfig config)
        {
            Operate operate;
            if (operateStr.IsNotNullAndEmpty() && Enum.TryParse<Operate>(operateStr, true, out var value))
            {
                operate = value;
            }
            else
            {
                operate = Operate.Skip;
            }

            if (config.dict.TryGetValue(operate, out var item))
            {
                return item;
            }

            item = new Item();
            config.dict[operate] = item;
            return item;
        }

        /// <summary>
        /// 对表字段进行处理
        /// </summary>
        /// <param name="tables"></param>
        public void Handle(List<TableInfo> tables)
        {
            if (dict.TryGetValue(Operate.Skip, out var item))
            {
                if (item.Count > 0)
                {
                    tables.RemoveAll(t => item.EqualsFilter.Any(x => t.Name.Equals(x))
                                       || item.StartsWithFilter.Any(x => t.Name.StartsWith(x))
                                       || item.EndsWithFilter.Any(x => t.Name.EndsWith(x))
                                       || item.ContainsFilter.Any(x => t.Name.Contains(x)));
                }
            }

            if (dict.TryGetValue(Operate.Trim, out item))
            {
                if (item.StartsWithFilter.Count > 0)
                {
                    foreach (var temp in item.StartsWithFilter)
                    {
                        Handle(temp, tables, StartsWithHandle);
                    }
                }
                if (item.EndsWithFilter.Count > 0)
                {
                    foreach (var temp in item.EndsWithFilter)
                    {
                        Handle(temp, tables, EndsWithHandle);
                    }
                }
            }
        }

        private void Handle(string key, List<TableInfo> tables, NameHandle match)
        {
            foreach (var table in tables)
            {
                if (match(table.Name, key, out var entityName))
                {
                    table.EntityName = entityName;
                }
            }
        }

        private bool StartsWithHandle(string name, string key, out string entityName)
        {
            if (name.StartsWith(key))
            {
                entityName = name.Substring(key.Length);
                return true;
            }

            entityName = null;
            return false;
        }

        private bool EndsWithHandle(string name, string key, out string entityName)
        {
            if (name.EndsWith(key))
            {
                entityName = name.Substring(0, name.Length - key.Length);
                return true;
            }

            entityName = null;
            return false;
        }

        enum Operate
        {
            /// <summary>
            /// 跳过
            /// </summary>
            Skip,

            /// <summary>
            /// 移除前导匹配项或尾部匹配项
            /// </summary>
            Trim,
        }

        class Item
        {
            /// <summary>
            /// 相等过滤
            /// </summary>
            public List<string> EqualsFilter { get; set; }

            /// <summary>
            /// 匹配开头过滤
            /// </summary>
            public List<string> StartsWithFilter { get; set; }

            /// <summary>
            /// 匹配结尾过滤
            /// </summary>
            public List<string> EndsWithFilter { get; set; }

            /// <summary>
            /// 包含过滤
            /// </summary>
            public List<string> ContainsFilter { get; set; }

            /// <summary>
            /// 数量
            /// </summary>
            public int Count => EqualsFilter.Count + StartsWithFilter.Count + EndsWithFilter.Count + ContainsFilter.Count;

            public Item()
            {
                EqualsFilter = new List<string>();
                StartsWithFilter = new List<string>();
                EndsWithFilter = new List<string>();
                ContainsFilter = new List<string>();
            }
        }
    }
}
