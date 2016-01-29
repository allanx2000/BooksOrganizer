using BooksOrganizer.Models;
using Innouvous.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksOrganizer.ViewModels
{
    public class TreeNode : ViewModel45
    {
        public Type GetDataType()
        {
            return data.GetType();
        }

        public bool IsBook()
        {
            return data is Book;
        }

        public bool IsNote()
        {
            return data is Note;
        }
        
        public enum NodeType
        {
            Node,
            Leaf
        }

        private INodeData data;
        private ObservableCollection<TreeNode> nodes;

        public ObservableCollection<TreeNode> Nodes
        {
            get
            {
                return nodes;
            }
        }
        public NodeType Type { get; private set; }

        private bool isExpanded;
        public bool IsExpanded {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                RaisePropertyChanged("IsExpanded");
            }
        }

        public INodeData GetData()
        {
            return data;
        }

        private string text;
        public string Text
        {
            get { return text;  }
            set
            {
                text = value;
                RaisePropertyChanged("Text");
            }
        }

        public TreeNode(NodeType type, INodeData data, string text)
        {
            this.data = data;

            Type = type;
            Text = text;

            if (type == NodeType.Node)
            {
                nodes = new ObservableCollection<TreeNode>();
            }
        }

        public class IsNotNodeException : Exception
        {
            public IsNotNodeException() :base("This is not a node")
            {

            }
        }

        public void Add(TreeNode child)
        {
            if (Type != NodeType.Node)
                throw new IsNotNodeException();
            else
                nodes.Add(child);
        }

        public List<TreeNode> GetNodes()
        {
            if (Type != NodeType.Node)
                throw new IsNotNodeException();
            else
                return nodes.ToList();
        }

        public void ClearChildren()
        {
            if (Type != NodeType.Node)
                throw new IsNotNodeException();
            else
            {
                Nodes.Clear();
            }

        }
    }
}
