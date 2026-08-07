using System;
using System.Collections.Generic;

namespace OrzAutoEntity.Modes
{
    public class TableInfo
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// 表名/视图名
        /// </summary>
        public string Name { get; set; }

        private string entityName;
        /// <summary>
        /// 实体名
        /// </summary>
        public string EntityName
        {
            get { return entityName ?? Name; }
            set { entityName = value; }
        }

        /// <summary>
        /// 小驼峰格式的实体名
        /// </summary>
        public string LowerCamelName => EntityName.GetLowerCamelCaseName();

        /// <summary>
        /// 大驼峰格式的实体名
        /// </summary>
        public string CamelName => EntityName.GetCamelCaseName();

        /// <summary>
        /// 小写格式的实体名
        /// </summary>
        public string LowerName => EntityName.ToLower();

        /// <summary>
        /// 是否是视图
        /// </summary>
        public bool IsView { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// sql
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        /// 字段信息
        /// </summary>
        public List<ColumnInfo> Columns { get; set; }
    }
}
