using System;
using System.Collections.Generic;
using System.Linq;

namespace OrzAutoEntity.Modes
{
    internal class DisplayHandle
    {
        /// <summary>
        /// 更新列表，已生成的实体
        /// </summary>
        public List<TableFileInfo> UpdateList { get; set; }

        /// <summary>
        /// 新增列表，未生成的实体
        /// </summary>
        public List<TableFileInfo> NewList { get; set; }

        /// <summary>
        /// 删除列表，数据库中不存在的实体
        /// </summary>
        public List<string> DeleteList { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="models"></param>
        public void Init(List<TableFileInfo> tables, List<string> models)
        {
            UpdateList = tables.Where(t => models.Any(x => x.Equals(t.FilePath, StringComparison.OrdinalIgnoreCase))).OrderBy(t => t.FilePath).ToList();
            NewList = tables.Where(t => models.All(x => x.Equals(t.FilePath, StringComparison.OrdinalIgnoreCase) == false)).OrderBy(t => t.FilePath).ToList();
            DeleteList = models.Where(x => tables.All(t => x.Equals(t.FilePath, StringComparison.OrdinalIgnoreCase) == false)).OrderBy(t => t).ToList();
        }

        /// <summary>
        /// 从删除列表里移除
        /// </summary>
        /// <param name="removeList"></param>
        public void RemoveDeleteList(List<string> removeList)
        {
            DeleteList = DeleteList.Except(removeList).ToList();
        }

        /// <summary>
        /// 筛选删除列表
        /// </summary>
        /// <param name="input">筛选值</param>
        /// <returns></returns>
        public IEnumerable<string> FilterDeleteList(string input = null)
        {
            if (input.IsNullOrEmpty())
            {
                foreach (var item in DeleteList)
                {
                    yield return item;
                }
            }
            else
            {
                var result = DeleteList.Where(t => t.IndexOf(input, StringComparison.OrdinalIgnoreCase) > -1);
                foreach (var item in result)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// 筛选更新列表
        /// </summary>
        /// <param name="input">筛选值</param>
        /// <returns></returns>
        public List<TableFileInfo> FilterUpdateList(string input = null)
        {
            return Filter(input, UpdateList);
        }

        /// <summary>
        /// 筛选新增列表
        /// </summary>
        /// <param name="input">筛选值</param>
        /// <returns></returns>
        public List<TableFileInfo> FilterNewList(string input = null)
        {
            return Filter(input, NewList);
        }

        private List<TableFileInfo> Filter(string input, List<TableFileInfo> data)
        {
            return input.IsNullOrEmpty() ? data : data.Where(t => t.FilePath.IndexOf(input, StringComparison.OrdinalIgnoreCase) > -1).ToList();
        }
    }
}
