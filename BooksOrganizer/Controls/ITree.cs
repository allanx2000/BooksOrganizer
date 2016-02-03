using System;
using System.Collections.Generic;
using BooksOrganizer.Models;
using BooksOrganizer.ViewModels;

namespace BooksOrganizer.Controls
{
    public interface ITree
    {
        Action OnSelectedChanged { get; set; }
        INodeData SelectedNodeData { get; }
        void RefreshTree(bool retainSelected);
        void SetGenerateTree(Func<ICollection<TreeNode>> generateTree);
    }
}