using System;
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
            var project = GetSelectedProject();
            return project == null ? string.Empty : project.Properties.Item("FullPath").Value.ToString();
        }
    }
}
