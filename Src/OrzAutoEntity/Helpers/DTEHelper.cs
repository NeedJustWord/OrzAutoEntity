using System;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace OrzAutoEntity.Helpers
{
    static class DTEHelper
    {
        #region DTE2
        private static DTE2 dte2;
        internal static DTE2 DTE2
        {
            get
            {
                if (dte2 == null)
                {
                    dte2 = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE2;
                }
                return dte2;
            }
        }
        #endregion

        /// <summary>
        /// 获取选中的第一个项目
        /// </summary>
        /// <returns></returns>
        public static Project GetSelectedProject()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var items = (Array)DTE2.ToolWindows.SolutionExplorer.SelectedItems;
            foreach (UIHierarchyItem selItem in items)
            {
                if (selItem.Object is Project item && item != null) return item;
            }
            return null;
        }

        /// <summary>
        /// 获取选中的第一个项目的全路径
        /// </summary>
        /// <returns></returns>
        public static string GetSelectedProjectFullPath()
        {
            return GetProjectFullPath(GetSelectedProject());
        }

        /// <summary>
        /// 获取项目的全路径
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static string GetProjectFullPath(Project project)
        {
            return project == null ? string.Empty : project.Properties.Item("FullPath").Value.ToString();
        }

        /// <summary>
        /// 获取指定目录下的实体类
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static List<string> GetExistsEntities(string directory)
        {
            var result = new List<string>();
            var project = GetSelectedProject();
            if (project == null) return result;

            var items = project.ProjectItems;
            if (!string.IsNullOrEmpty(directory))
            {
                var notFind = true;
                foreach (ProjectItem item in items)
                {
                    if (item.Name.Equals(directory, StringComparison.OrdinalIgnoreCase))
                    {
                        items = item.ProjectItems;
                        notFind = false;
                        break;
                    }
                }
                if (notFind) return result;
            }

            foreach (ProjectItem item in items)
            {
                if (item.Name.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(item.Name.Substring(0, item.Name.Length - ".cs".Length));
                }
            }

            return result;
        }

        /// <summary>
        /// 移除指定目录下的文件
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fileNames"></param>
        public static void RemoveFiles(string directory, List<string> fileNames)
        {
            var project = GetSelectedProject();
            if (project == null) return;

            var items = project.ProjectItems;
            if (!string.IsNullOrEmpty(directory))
            {
                var notFind = true;
                foreach (ProjectItem item in items)
                {
                    if (item.Name.Equals(directory, StringComparison.OrdinalIgnoreCase))
                    {
                        items = item.ProjectItems;
                        notFind = false;
                        break;
                    }
                }
                if (notFind) return;
            }

            foreach (var fileName in fileNames)
            {
                var fileNameWithExt = $"{fileName}.cs";
                foreach (ProjectItem item in items)
                {
                    if (item.Name.Equals(fileNameWithExt, StringComparison.OrdinalIgnoreCase))
                    {
                        item.Remove();
                    }
                }
            }
        }
    }
}
