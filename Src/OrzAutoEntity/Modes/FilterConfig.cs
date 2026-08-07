using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace OrzAutoEntity.Modes
{
    public class FilterConfig
    {
        public string Id { get; set; }

        public FilterType Type { get; set; }

        private Dictionary<string, Item> dict;
        private delegate bool NameHandle(string name, string key, out string entityName);

        public FilterConfig(string id, FilterType type)
        {
            Id = id;
            Type = type;
            dict = new Dictionary<string, Item>();
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
                var type = Enum.TryParse<FilterType>(node.GetAttribute("type"), true, out var value) ? value : FilterType.Table;
                var config = new FilterConfig(node.GetAttribute("id"), type);
                foreach (XmlElement element in node.SelectNodes("Item"))
                {
                    var filter = element.InnerText.Trim();
                    var item = GetItem(element.GetAttribute("operate"), element.GetAttribute("value"), config);
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

        private static Item GetItem(string operateStr, string valueStr, FilterConfig config)
        {
            FilterOperate operate;
            if (operateStr.IsNotNullAndEmpty() && Enum.TryParse<FilterOperate>(operateStr, true, out var value))
            {
                operate = value;
            }
            else
            {
                operate = FilterOperate.Skip;
            }

            var key = operate == FilterOperate.SetPath || operate == FilterOperate.SetEntityName ? $"{GetOperateKeyPrefix(operate)}{valueStr}" : operate.ToString();
            if (config.dict.TryGetValue(key, out var item))
            {
                return item;
            }

            item = new Item();
            config.dict[key] = item;
            return item;
        }

        private static string GetOperateKeyPrefix(FilterOperate operate)
        {
            return $"{operate.ToString()}:";
        }

        /// <summary>
        /// 对实体类进行处理
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="tables"></param>
        public static void Handle(List<FilterConfig> filters, List<string> modelPaths)
        {
            foreach (var filter in filters)
            {
                filter.Handle(modelPaths);
            }
        }

        private void Handle(List<string> modelPaths)
        {
            if (dict.TryGetValue(FilterOperate.Skip.ToString(), out var item))
            {
                if (item.Count > 0)
                {
                    modelPaths.RemoveAll(t => item.IsMatch(Path.GetFileName(t)));
                }
            }

            if (dict.TryGetValue(FilterOperate.Include.ToString(), out item))
            {
                if (item.Count > 0)
                {
                    modelPaths.RemoveAll(t => item.IsMatch(Path.GetFileName(t)) == false);
                }
            }
        }

        /// <summary>
        /// 对表进行处理
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="tables"></param>
        public static void Handle(List<FilterConfig> filters, List<TableInfo> tables)
        {
            foreach (var filter in filters)
            {
                filter.Handle(tables);
            }
        }

        private void Handle(List<TableInfo> tables)
        {
            if (dict.TryGetValue(FilterOperate.Skip.ToString(), out var item))
            {
                if (item.Count > 0)
                {
                    tables.RemoveAll(t => item.IsMatch(t.Name));
                }
            }

            if (dict.TryGetValue(FilterOperate.Include.ToString(), out item))
            {
                if (item.Count > 0)
                {
                    tables.RemoveAll(t => item.IsMatch(t.Name) == false);
                }
            }

            if (dict.TryGetValue(FilterOperate.Trim.ToString(), out item))
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

            var key = GetOperateKeyPrefix(FilterOperate.SetPath);
            foreach (var keyValue in dict.Where(t => t.Key.StartsWith(key)))
            {
                var path = keyValue.Key.Substring(key.Length);
                item = keyValue.Value;
                foreach (var table in tables)
                {
                    if (item.IsMatch(table.Name))
                    {
                        table.FilePath = path;
                    }
                }
            }

            key = GetOperateKeyPrefix(FilterOperate.SetEntityName);
            foreach (var keyValue in dict.Where(t => t.Key.StartsWith(key)))
            {
                var entityName = keyValue.Key.Substring(key.Length);
                item = keyValue.Value;
                foreach (var table in tables)
                {
                    if (item.IsMatch(table.Name))
                    {
                        table.EntityName = entityName;
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

        enum FilterOperate
        {
            /// <summary>
            /// 跳过
            /// </summary>
            Skip,

            /// <summary>
            /// 包含
            /// </summary>
            Include,

            /// <summary>
            /// 移除前导匹配项或尾部匹配项
            /// </summary>
            Trim,

            /// <summary>
            /// 设置路径
            /// </summary>
            SetPath,

            /// <summary>
            /// 设置实体名
            /// </summary>
            SetEntityName,
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

            public bool IsMatch(string name)
            {
                return EqualsFilter.Any(x => name.Equals(x, StringComparison.OrdinalIgnoreCase))
                    || StartsWithFilter.Any(x => name.StartsWith(x, StringComparison.OrdinalIgnoreCase))
                    || EndsWithFilter.Any(x => name.EndsWith(x, StringComparison.OrdinalIgnoreCase))
                    || ContainsFilter.Any(x => name.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }
    }

    public enum FilterType
    {
        /// <summary>
        /// 表
        /// </summary>
        Table,

        /// <summary>
        /// 实体类
        /// </summary>
        Model,
    }
}
