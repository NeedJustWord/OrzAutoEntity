using System.Data;
using System.Globalization;

namespace System
{
    static class Extension
    {
        /// <summary>
        /// 转换为驼峰命名风格
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetCamelCaseName(this string name)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower()).Replace("_", "");
        }

        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue">为null时的默认值</param>
        /// <returns></returns>
        public static string AsString(this object obj, string defaultValue = "")
        {
            return obj == null ? defaultValue : obj.ToString();
        }

        /// <summary>
        /// 设置命令参数
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="param"></param>
        public static void SetParameters(this IDbCommand cmd, object param)
        {
            cmd.Parameters.Clear();

            if (param == null) return;

            var t = param.GetType();
            var paramInfos = t.GetProperties();
            foreach (var pi in paramInfos)
            {
                AddParameter(cmd, pi.Name, pi.GetValue(param, null));
            }
        }

        private static void AddParameter(IDbCommand cmd, string name, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }
    }
}
