using BooksOrganizer.Models;
using BooksOrganizer.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BooksOrganizer.Controls
{
    /// <summary>
    /// Interaction logic for Tree.xaml
    /// </summary>
    public partial class Tree : UserControl, ITree
    {
        private readonly ObservableCollection<TreeNode> tree = new ObservableCollection<TreeNode>();

        public Tree()
        {
            InitializeComponent();
            TreeList.ItemsSource = tree;

        }

        public void SetGenerateTree(Func<ICollection<TreeNode>> generateTree)
        {
            this.GenerateTree = generateTree;

            RefreshTree(false);
        }

        public INodeData SelectedNodeData { get; private set; }

        public Action OnSelectedChanged { get; set; }

        private void TreeList_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedNode = e.NewValue as TreeNode;

            if (selectedNode != null)
                SelectedNodeData = selectedNode.GetData();
            else
                SelectedNodeData = null;

            if (OnSelectedChanged != null)
                OnSelectedChanged.Invoke();
        }

        private Func<ICollection<TreeNode>> GenerateTree;

        public void RefreshTree(bool retainSelected)
        {
            var data = GenerateTree();

            tree.Clear();
            foreach (TreeNode n in data)
            {
                tree.Add(n);
            }

            if (retainSelected)
            {
                TreeNode found = FindNode(SelectedNodeData, tree);
                while (found != null)
                {
                    found.IsExpanded = true;
                    found = found.Parent;
                }

            }
        }

        private TreeNode FindNode(INodeData toMatch, ICollection<TreeNode> nodes)
        {
            if (toMatch == null || nodes == null)
                return null;

            foreach (TreeNode n in nodes)
            {
                if (n.GetData() == toMatch)
                    return n;
                else if (n.Type == TreeNode.NodeType.Node)
                {
                    TreeNode found = FindNode(toMatch, n.Nodes);
                    return found;
                }
            }

            return null;
        }
    }
}
