using System;
using System.Collections.Generic;
using System.Xml;
using OrzAutoEntity.DataAccess;

namespace OrzAutoEntity.Helpers
{
    public static class TypeMapping
    {
        public struct TypeMappingConfig
        {
            public string ClrType { get; set; }
            public bool IsNumber { get; set; }

            public TypeMappingConfig(string clrType, bool isNumber)
            {
                ClrType = clrType;
                IsNumber = isNumber;
            }
        }

        public static Dictionary<DatabaseType, Dictionary<string, TypeMappingConfig>> Mapping { get; private set; } = new Dictionary<DatabaseType, Dictionary<string, TypeMappingConfig>>();

        public static void Reload(XmlDocument doc)
        {
            Mapping.Clear();
            var nodes = doc.SelectNodes("AutoEntity/TypeMapping/Mapping");
            foreach (XmlElement node in nodes)
            {
                if (Enum.TryParse<DatabaseType>(node.GetAttribute("name"), out var dbType) == false) continue;

                var dict = new Dictionary<string, TypeMappingConfig>(StringComparer.OrdinalIgnoreCase);
                foreach (XmlElement item in node.SelectNodes("Item"))
                {
                    var clrType = item.GetAttribute("clrType").Trim();
                    var isNumber = item.GetAttribute("isNumber").Trim().AsBool();
                    foreach (var sqlType in item.GetAttribute("sqlType").SplitRemoveEmptyEntries('|'))
                    {
                        dict[sqlType.Trim()] = new TypeMappingConfig(clrType, isNumber);
                    }
                }
                Mapping[dbType] = dict;
            }
        }

        /// <summary>
        /// 获取字段对应的CLR类型
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="sqlType"></param>
        /// <returns></returns>
        public static string GetClrType(DatabaseType dbType, string sqlType)
        {
            if (Mapping.TryGetValue(dbType, out var dict) == false)
            {
                throw new Exception("未知的数据库映射类型");
            }

            return dict.TryGetValue(sqlType, out var config) ? config.ClrType : $"undefine_db_type({sqlType})";
        }

        /// <summary>
        /// 获取字段是否是数字
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="sqlType"></param>
        /// <returns></returns>
        public static bool IsNumber(DatabaseType dbType, string sqlType)
        {
            if (Mapping.TryGetValue(dbType, out var dict) == false)
            {
                throw new Exception("未知的数据库映射类型");
            }

            return dict.TryGetValue(sqlType, out var config) && config.IsNumber;
        }
    }
}
