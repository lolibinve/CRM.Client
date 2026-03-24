using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Aipark.Wpf.Controls.Extensions
{
    public static class TreeViewExtensions
    {
        /// <summary>
        /// 展开所有子节点
        /// </summary>
        /// <param name="treeView"></param>
        public static void ExpandAllNodes(this TreeView treeView)
        {
            if (treeView.ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                treeView.UpdateLayout();
            }

            foreach (var item in treeView.Items)
            {
                TreeViewItem treeViewItem = treeView.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                treeViewItem.ExpandSubtree();
            }
        }


        /// <summary>
        /// 收缩所有子节点
        /// </summary>
        /// <param name="treeView"></param>
        public static void CollapseAllNodes(this TreeView treeView)
        {
            if (treeView.ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                treeView.UpdateLayout();
            }

            foreach (var item in treeView.Items)
            {
                TreeViewItem treeViewItem = treeView.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                CollapseSubtree(treeViewItem);
            }
        }
        /// <summary>
        /// 递归收缩子节点
        /// </summary>
        /// <param name="container"></param>
        private static void CollapseSubtree(TreeViewItem container)
        {
            container.IsExpanded = false;
            if (container.ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                container.UpdateLayout();
            }

            foreach (var item in container.Items)
            {
                TreeViewItem treeViewItem = container.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;

                treeViewItem.IsExpanded = false;

                if (treeViewItem.HasItems)
                {
                    CollapseSubtree(treeViewItem);
                }
            }
        }


        /// <summary>
        /// 强制所有节点正确布局,解决默认未展开节子元素获取值null的问题
        /// </summary>
        /// <param name="treeView"></param>
        public static void UpdateContainersLayout(this TreeView treeView)
        {
            if (treeView.ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                treeView.UpdateLayout();
            }

            Dictionary<TreeViewItem, bool> hasExpandDictionary = new Dictionary<TreeViewItem, bool>();

            foreach (var item in treeView.Items)
            {
                if (treeView.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem treeViewItem)
                {
                    GetTreeNodes(treeViewItem, hasExpandDictionary);
                }
            }

            //从内往外恢复之前的状态
            TreeViewItem[] treeViewItems = hasExpandDictionary.Keys.ToArray();
            for (int i = treeViewItems.Length - 1; i > 0; i--)
            {
                treeViewItems[i].IsExpanded = hasExpandDictionary[treeViewItems[i]];
            }
        }
        /// <summary>
        /// 递归获取所有节点及其子节点信息
        /// 处理ContainerFromItem为null：https://www.cnblogs.com/lclc88com/archive/2012/11/30/2796116.html
        /// </summary>
        /// <param name="container"></param>
        /// <param name="hasExpandDictionary"></param>
        private static void GetTreeNodes(TreeViewItem container, Dictionary<TreeViewItem, bool> hasExpandDictionary)
        {
            if (!hasExpandDictionary.ContainsKey(container))
            {
                hasExpandDictionary.Add(container, container.IsExpanded);
            }

            container.IsExpanded = true;
            if (container.ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                container.UpdateLayout();
            }

            foreach (var item in container.Items)
            {
                if ((container.ItemContainerGenerator.ContainerFromItem(item) as DependencyObject) is TreeViewItem treeViewItem)
                {
                    GetTreeNodes(treeViewItem, hasExpandDictionary);
                }
            }
        }


        /// <summary>
        /// 以深度优先方式遍历并获取所有节点列表
        /// </summary>
        /// <param name="treeView"></param>
        public static List<TreeViewItem> GetAllNodes(this TreeView treeView)
        {
            treeView.UpdateContainersLayout();

            List<TreeViewItem> treeNodes = new List<TreeViewItem>();

            foreach (var item in treeView.Items)
            {
                if (treeView.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem treeViewItem)
                {
                    GetTreeNodes(treeViewItem, treeNodes);
                }
            }

            return treeNodes;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="list"></param>
        private static void GetTreeNodes(TreeViewItem container, List<TreeViewItem> list)
        {
            list.Add(container);

            foreach (var item in container.Items)
            {
                if ((container.ItemContainerGenerator.ContainerFromItem(item) as DependencyObject) is TreeViewItem treeViewItem)
                {
                    GetTreeNodes(treeViewItem, list);
                }
            }
        }
    }
}
