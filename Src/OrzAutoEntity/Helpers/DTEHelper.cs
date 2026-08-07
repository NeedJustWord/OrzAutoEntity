using System;
using System.Collections.Generic;
using System.IO;
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
            if (TryGetProjectItems(directory, out var items) == false)
            {
                return result;
            }

            GetExistsEntities(items, ref result);
            return result;
        }

        private static bool TryGetProjectItems(string directory, out ProjectItems items)
        {
            var project = GetSelectedProject();
            if (project == null)
            {
                items = null;
                return false;
            }

            items = project.ProjectItems;
            if (directory.IsNullOrEmpty())
            {
                return true;
            }

            var isFind = true;
            var parts = directory.PathSplit();
            foreach (var part in parts)
            {
                var findPart = TryFindProjectItem(items, part, out var findItem);

                if (findPart == false)
                {
                    isFind = false;
                    break;
                }

                items = findItem.ProjectItems;
            }

            return isFind;
        }

        private static void GetExistsEntities(ProjectItems items, ref List<string> result)
        {
            var queue = new Queue<Tuple<string, ProjectItem>>(items.Count);
            foreach (ProjectItem item in items)
            {
                queue.Enqueue(new Tuple<string, ProjectItem>(string.Empty, item));
            }

            while (queue.Count > 0)
            {
                var tuple = queue.Dequeue();
                var parentPath = tuple.Item1;
                var current = tuple.Item2;

                if (current.Kind == Constants.vsProjectItemKindPhysicalFolder)
                {
                    foreach (ProjectItem item in current.ProjectItems)
                    {
                        queue.Enqueue(new Tuple<string, ProjectItem>(Path.Combine(parentPath, current.Name), item));
                    }
                }
                else
                {
                    result.Add(Path.Combine(parentPath, current.Name));
                    foreach (ProjectItem item in current.ProjectItems)
                    {
                        queue.Enqueue(new Tuple<string, ProjectItem>(parentPath, item));
                    }
                }
            }
        }

        /// <summary>
        /// 查找<paramref name="name"/>是否存在
        /// </summary>
        /// <param name="items"></param>
        /// <param name="name"></param>
        /// <param name="findItem"></param>
        /// <returns></returns>
        public static bool TryFindProjectItem(ProjectItems items, string name, out ProjectItem findItem)
        {
            foreach (ProjectItem item in items)
            {
                if (item.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    findItem = item;
                    return true;
                }
            }

            findItem = null;
            return false;
        }
    }
}
