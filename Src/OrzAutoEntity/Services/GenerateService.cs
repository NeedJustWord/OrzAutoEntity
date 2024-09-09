using System.IO;
using NVelocity;
using NVelocity.App;
using OrzAutoEntity.Modes;

namespace OrzAutoEntity.Services
{
    public static class GenerateService
    {
        /// <summary>
        /// 生成实体类文件内容
        /// </summary>
        /// <param name="template">模板字符串</param>
        /// <param name="table">表信息</param>
        /// <returns></returns>
        public static string GetEntityContent(string template, TableInfo table)
        {
            var engine = new VelocityEngine();
            engine.Init();

            var writer = new StringWriter();
            var context = new VelocityContext();
            context.Put("Table", table);

            engine.Evaluate(context, writer, "entity", template);
            return writer.GetStringBuilder().ToString();
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="content">文件内容</param>
        public static void SaveFile(string path, string content)
        {
            File.WriteAllText(path, content);
        }
    }
}
