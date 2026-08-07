using System;
using System.Collections.Generic;
using System.IO;
using EnvDTE;
using OrzAutoEntity.Helpers;
using OrzAutoEntity.Services;

namespace OrzAutoEntity.Modes
{
    /// <summary>
    /// 树节点基类
    /// </summary>
    public abstract class TreeNode
    {
        #region 实体
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullPath { get; set; }

        public TreeNode(string name, string fullPath)
        {
            Name = name;
            FullPath = fullPath;
        }

        protected virtual void Add(TreeNode node)
        {
        }

        /// <summary>
        /// 生成文件
        /// </summary>
        /// <param name="projectFullPath">项目文件所在完整路径</param>
        public virtual void GenerateFile(string projectFullPath)
        {
        }

        /// <summary>
        /// 生成文件
        /// </summary>
        /// <param name="parentPath"></param>
        internal abstract void InternalGenerateFile(string parentPath);

        /// <summary>
        /// 将实体类添加到项目中
        /// </summary>
        /// <param name="project"></param>
        /// <param name="projectFullPath"></param>
        public virtual void AddToProject(Project project, string projectFullPath)
        {
        }

        /// <summary>
        /// 将实体类添加到项目中
        /// </summary>
        /// <param name="project"></param>
        /// <param name="modelPath"></param>
        internal abstract void InternalAddToProject(Project project, string modelPath);

        /// <summary>
        /// 将实体类从项目中移除
        /// </summary>
        /// <param name="project"></param>
        public virtual void RemoveFromProject(Project project)
        {
        }

        /// <summary>
        /// 将实体类从项目中移除
        /// </summary>
        /// <param name="project"></param>
        internal abstract void InternalRemoveFromProject(ProjectItems project);
        #endregion

        #region 静态
        /// <summary>
        /// 创建文件路径树形结构
        /// </summary>
        /// <param name="config"></param>
        /// <param name="infos"></param>
        /// <returns></returns>
        public static TreeNode CreateTreeNode(DatabaseConfig config, List<TableFileInfo> infos)
        {
            var path = config.Directory.PathFormat();
            var root = new PathNode(Path.GetFileName(path), path);
            var dict = new Dictionary<string, TreeNode>();

            foreach (var item in infos)
            {
                var parts = item.FilePath.PathSplit();
                var lastIndex = parts.Length - 1;
                TreeNode parent = null;
                TreeNode info;
                string name, fullPath;
                for (var i = 0; i < parts.Length; i++)
                {
                    name = parts[i];
                    fullPath = parent == null ? name : Path.Combine(parent.FullPath, name);
                    if (dict.TryGetValue(fullPath, out var value))
                    {
                        parent = value;
                        continue;
                    }

                    if (i == lastIndex)
                    {
                        info = new FileNode(name, fullPath, item);
                    }
                    else
                    {
                        info = new PathNode(name, fullPath);
                    }

                    if (parent == null)
                    {
                        root.Add(info);
                    }
                    else
                    {
                        parent.Add(info);
                    }

                    dict[fullPath] = info;
                    parent = info;
                }
            }

            return root;
        }
        #endregion
    }

    /// <summary>
    /// 路径节点
    /// </summary>
    public class PathNode : TreeNode
    {
        /// <summary>
        /// 子节点，为空时表示文件
        /// </summary>
        public List<TreeNode> Children { get; set; }

        public PathNode(string name, string fullPath) : base(name, fullPath)
        {
            Children = new List<TreeNode>();
        }

        protected override void Add(TreeNode node)
        {
            Children.Add(node);
        }

        public override void GenerateFile(string projectFullPath)
        {
            var path = Path.GetDirectoryName(Path.Combine(projectFullPath, FullPath));
            DirectoryHelper.CreateDirectory(path);
            InternalGenerateFile(path);
        }

        internal override void InternalGenerateFile(string parentPath)
        {
            var path = Path.Combine(parentPath, Name);
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            foreach (var item in Children)
            {
                item.InternalGenerateFile(path);
            }
        }

        public override void AddToProject(Project project, string projectFullPath)
        {
            InternalAddToProject(project, Path.Combine(projectFullPath, FullPath));
        }

        internal override void InternalAddToProject(Project project, string modelPath)
        {
            foreach (var item in Children)
            {
                item.InternalAddToProject(project, modelPath);
            }
        }

        public override void RemoveFromProject(Project project)
        {
            var searchItems = project.ProjectItems;
            var parts = Path.GetDirectoryName(FullPath).PathSplit();
            foreach (var part in parts)
            {
                if (DTEHelper.TryFindProjectItem(searchItems, part, out var findItem))
                {
                    searchItems = findItem.ProjectItems;
                }
                else
                {
                    return;
                }
            }

            InternalRemoveFromProject(searchItems);
        }

        internal override void InternalRemoveFromProject(ProjectItems project)
        {
            if (DTEHelper.TryFindProjectItem(project, Name, out var findItem))
            {
                foreach (var item in Children)
                {
                    item.InternalRemoveFromProject(findItem.ProjectItems);
                }
            }
        }
    }

    /// <summary>
    /// 文件节点
    /// </summary>
    public class FileNode : TreeNode
    {
        /// <summary>
        /// 表生成文件信息，文件时才有值
        /// </summary>
        public TableFileInfo TableFile { get; set; }

        public FileNode(string name, string fullPath, TableFileInfo tableFile) : base(name, fullPath)
        {
            TableFile = tableFile;
        }

        internal override void InternalGenerateFile(string parentPath)
        {
            var file = Path.Combine(parentPath, Name);
            var content = GenerateService.GetEntityContent(TableFile.Config.Content, TableFile.Table);
            GenerateService.SaveFile(file, content);
        }

        internal override void InternalAddToProject(Project project, string modelPath)
        {
            project.ProjectItems.AddFromFile(Path.Combine(modelPath, FullPath));
        }

        internal override void InternalRemoveFromProject(ProjectItems project)
        {
            if (DTEHelper.TryFindProjectItem(project, Name, out var findItem))
            {
                findItem.Remove();
            }
        }
    }
}
