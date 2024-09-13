using System;
using System.Collections.Generic;

namespace OrzAutoEntity.Modes
{
    public class TableInfo
    {
        /// <summary>
        /// 文件全路径
        /// </summary>
        public string FileFullPath { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 表名/视图名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 驼峰格式的表名/视图名
        /// </summary>
        public string CamelName => Name.GetCamelCaseName();

        /// <summary>
        /// 小写格式的表名/视图名
        /// </summary>
        public string LowerName => Name.ToLower();

        /// <summary>
        /// 是否是视图
        /// </summary>
        public bool IsView { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 字段信息
        /// </summary>
        public List<ColumnInfo> Columns { get; set; }
    }
}
