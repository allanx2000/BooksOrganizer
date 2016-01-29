using BooksOrganizer.Models;
using Innouvous.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BooksOrganizer.ViewModels
{
    public class WorkspaceViewModel : ViewModel
    {
        #region GroupBy

        public enum GroupBy
        {
            Title,
            Topic
        }
        private GroupBy selectedGroupBy;
        public GroupBy SelectedGroupBy
        {
            get
            {
                return selectedGroupBy;
            }
            set
            {
                selectedGroupBy = value;
                RaisePropertyChanged("SelectedGroupBy");
                RaisePropertyChanged("ByTitle");
                RaisePropertyChanged("ByTopic");

            }
        }
        public bool ByTitle
        {
            get
            {
                return selectedGroupBy == GroupBy.Title;
            }
            set
            {
                SelectedGroupBy = GroupBy.Title;
            }
        }
        public bool ByTopic
        {
            get
            {
                return selectedGroupBy == GroupBy.Topic;
            }
            set
            {
                SelectedGroupBy = GroupBy.Topic;
            }
        }
        #endregion

        #region OrderBy
        public enum OrderBy
        {
            Location,
            Created,
            Updated
        }
        
        private OrderBy selectedOrderBy;
        public bool ByUpdated
        {
            get
            {
                return selectedOrderBy == OrderBy.Updated;
            }
            set
            {
                selectedOrderBy = OrderBy.Updated;
            }
        }
        public bool ByCreated
        {
            get
            {
                return selectedOrderBy == OrderBy.Created;
            }
            set
            {
                selectedOrderBy = OrderBy.Created;
            }
        }

        public bool ByLocation
        {
            get
            {
                return selectedOrderBy == OrderBy.Location;
            }
            set
            {
                selectedOrderBy = OrderBy.Location;
            }
        }
        
        #endregion

        public bool ShowNotes { get; set; }
        public bool ExcludePublish { get; set; }

        public ObservableCollection<TreeNode> Tree { get; set; }
            
        //TODO: Change to Node?
        //Import from Analyst
        public object SelectedNode { get; internal set; }



        private WorkspaceWindow window;

        public WorkspaceViewModel(WorkspaceWindow window)
        {
            this.window = window;

            Tree = new ObservableCollection<TreeNode>();
            //Tree.Add(new TreeNode(TreeNode.NodeType.Node, null, "Root"));
            RaisePropertyChanged("Tree");
        }

        public ICommand TopicsManagerCommand
        {
            get
            {
                return new CommandHelper(ShowTopicsManager);
            }
        }

        public void ShowTopicsManager()
        {
            var window = new TopicsManager();
            window.ShowDialog();
        }

        public ICommand AddBookCommand
        {
            get
            {
                return new CommandHelper(AddBook);
            }
        }
        

        private void AddBook()
        {
            var window = new EditBookWindow();

            window.ShowDialog();
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new CommandHelper(SetFilter);
            }
        }
        
        private void SetFilter()
        {
            Tree.Clear();

            if (SelectedGroupBy == GroupBy.Title)
            {
                foreach (Book b in Workspace.Current.DB.Books.OrderBy(x => x.Title))
                {
                    var tn = new TreeNode(TreeNode.NodeType.Node, b, b.Title);
                    //tn.Add(new TreeNode(TreeNode.NodeType.Leaf, null, "TEST 2"));
                    Tree.Add(tn);
                }
            }
            else if (SelectedGroupBy == GroupBy.Topic)
            {
                foreach (Topic t in Workspace.Current.DB.Topics.OrderBy(x => x.Name))
                {
                    var tn = new TreeNode(TreeNode.NodeType.Node, t, t.Name);
                    Tree.Add(tn);
                }
            }

            RaisePropertyChanged("Tree");
        }
    }
}
