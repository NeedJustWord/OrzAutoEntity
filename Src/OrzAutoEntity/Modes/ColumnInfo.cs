using System;

namespace OrzAutoEntity.Modes
{
    public struct ColumnInfo
    {
        /// <summary>
        /// 列名
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
        /// 驼峰格式的列名
        /// </summary>
        public string CamelName => Name.GetCamelCaseName();

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbType { get; set; }

        /// <summary>
        /// CLR类型
        /// </summary>
        public string ClrType { get; set; }

        /// <summary>
        /// 模板类型
        /// </summary>
        public string Type => AllowNull && ClrType != "string" && ClrType != "byte[]" ? $"{ClrType}?" : ClrType;

        /// <summary>
        /// 长度，数字时表示整数位长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 小数位长度，针对数字有效
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// 是否允许空
        /// </summary>
        public bool AllowNull { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsKey { get; set; }

        /// <summary>
        /// 是否自增
        /// </summary>
        public bool Identity { get; set; }
    }
}
