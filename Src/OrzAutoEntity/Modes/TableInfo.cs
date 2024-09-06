using System.Collections.Generic;

namespace OrzAutoEntity.Modes
{
    public class TableInfo
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }

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
        public List<ColumnInfo> ColumnInfos { get; set; }
    }
}
