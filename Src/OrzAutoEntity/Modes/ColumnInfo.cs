using System;

namespace OrzAutoEntity.Modes
{
    public class ColumnInfo
    {
        /// <summary>
        /// 表名/视图名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 驼峰格式的列名
        /// </summary>
        public string CamelName => Name.GetCamelCaseName();

        /// <summary>
        /// 小写格式的列名
        /// </summary>
        public string LowerName => Name.ToLower();

        /// <summary>
        /// 注释
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbType { get; set; }

        /// <summary>
        /// CLR类型
        /// </summary>
        public string ClrType { get; set; }

        /// <summary>
        /// 表字段类型
        /// </summary>
        public string TypeInTable => AllowNull && NeedNullable(ClrType) ? $"{ClrType}?" : ClrType;

        /// <summary>
        /// 视图字段类型
        /// <para>有些数据库版本里视图的AllowNull不准(例如达梦1-2-38-21.07.15-143663-10018-ENT  Pack2)</para>
        /// </summary>
        public string TypeInView => NeedNullable(ClrType) ? $"{ClrType}?" : ClrType;

        /// <summary>
        /// 长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 数字精度
        /// </summary>
        public int Precision { get; set; }

        /// <summary>
        /// 数字标度
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

        private static bool NeedNullable(string clrType)
        {
            return clrType != "string" && clrType != "byte[]";
        }
    }
}
