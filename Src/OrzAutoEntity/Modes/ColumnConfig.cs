using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace OrzAutoEntity.Modes
{
    /// <summary>
    /// 字段配置
    /// </summary>
    public class ColumnConfig
    {
        /// <summary>
        /// 字段配置id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 表名，非空时针对指定表，为空时针对所有表
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 表配置项列表
        /// </summary>
        private List<Item> items;

        public ColumnConfig(string id, string tableName)
        {
            Id = id;
            TableName = tableName;
            items = new List<Item>();
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ColumnConfig> Reload(XmlDocument doc)
        {
            var result = new List<ColumnConfig>();
            var nodes = doc.SelectNodes("AutoEntity/Columns/Column");
            foreach (XmlElement node in nodes)
            {
                var config = new ColumnConfig(node.GetAttribute("id"), node.GetAttribute("tableName"));
                foreach (XmlElement item in node.SelectNodes("Item"))
                {
                    config.items.Add(new Item
                    {
                        ColumnName = item.GetAttribute("columnName"),
                        Skip = item.GetAttribute("skip").AsBool(),
                        ClrType = item.GetAttribute("clrType"),
                        FieldName = item.GetAttribute("fieldName"),
                    });
                }
                result.Add(config);
            }
            return result;
        }

        /// <summary>
        /// 对表字段进行处理
        /// </summary>
        /// <param name="tables"></param>
        public void Handle(List<TableInfo> tables)
        {
            if (TableName.IsNotNullAndEmpty())
            {
                var table = tables.FirstOrDefault(t => t.Name == TableName);
                if (table != null)
                {
                    Handle(table);
                }
            }
            else
            {
                foreach (var table in tables)
                {
                    Handle(table);
                }
            }
        }

        private void Handle(TableInfo table)
        {
            for (int i = table.Columns.Count - 1; i >= 0; i--)
            {
                var column = table.Columns[i];
                var item = items.FirstOrDefault(t => t.ColumnName == column.Name);
                if (item == null) continue;

                if (item.Skip)
                {
                    table.Columns.RemoveAt(i);
                    continue;
                }

                if (item.ClrType.IsNotNullAndEmpty())
                {
                    column.ClrType = item.ClrType;
                }

                if (item.FieldName.IsNotNullAndEmpty())
                {
                    column.FieldName = item.FieldName;
                }
            }
        }

        /// <summary>
        /// 字段配置项
        /// </summary>
        class Item
        {
            /// <summary>
            /// 字段名
            /// </summary>
            public string ColumnName { get; set; }

            /// <summary>
            /// 生成实体类时是否跳过
            /// </summary>
            public bool Skip { get; set; }

            /// <summary>
            /// 字段类型
            /// </summary>
            public string ClrType { get; set; }

            /// <summary>
            /// 字段名
            /// </summary>
            public string FieldName { get; set; }
        }
    }
}
