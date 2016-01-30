using BooksOrganizer.Models;
using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BooksOrganizer.ViewModels
{
    public class WorkspaceViewModel : ViewModel45
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

        private TreeNode selectedNode;

        public TreeNode SelectedNode {
            get { return selectedNode; }
            set
            {
                selectedNode = value;

                //TODO: Nicer?
                ((CommandHelper)editCommand).RaiseCanExecuteChanged();
                RaisePropertyChanged();
            }
        }



        private WorkspaceWindow window;

        public WorkspaceViewModel(WorkspaceWindow window)
        {
            this.window = window;

            editCommand = new CommandHelper(Edit, (o) => selectedNode != null);


            Tree = new ObservableCollection<TreeNode>();
            //Tree.Add(new TreeNode(TreeNode.NodeType.Node, null, "Root"));
            //RaisePropertyChanged("Tree");
        }

        public ICommand DeleteCommand
        {
            get
            {
                return new CommandHelper(DeleteSelected);
            }
        }

        private void DeleteSelected()
        {
            try
            {
                if (SelectedNode == null)
                {
                    MessageBoxFactory.ShowError("No item selected");
                    return;
                }

                if (SelectedNode.IsBook)
                    Util.DB.Books.Remove((Book)SelectedNode.GetData());
                else if (SelectedNode.IsNote)
                    Util.DB.Notes.Remove((Note)SelectedNode.GetData());
                else if (SelectedNode.IsTopic)
                    Util.DB.Topics.Remove((Topic)SelectedNode.GetData());
                else
                    throw new Exception("Type not supported: " + SelectedNode.GetDataType());

                Util.DB.SaveChanges();
                SetFilter();
            }
            catch (Exception e)
            {
                Util.DB.RejectChanges();
                MessageBoxFactory.ShowError(e);
            }
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

        private readonly ICommand editCommand;
        public ICommand EditCommand
        {
            get
            {
                return editCommand;
            }

        }



        private void Edit()
        {
            if (selectedNode == null)
                return;

            Window window;

            if (selectedNode.IsBook)
            {
                window = new EditBookWindow((Book)selectedNode.GetData());
                window.ShowDialog();

                SetFilter(window);
            }
        }
        
        public ICommand RefreshCommand
        {
            get
            {
                return new CommandHelper(SetFilter);
            }
        }

        /// <summary>
        /// Helper for avoiding refresh if Window is ICancellable and Cancelled
        /// </summary>
        /// <param name="window"></param>
        private void SetFilter(Window window)
        {
            if (window is ICancellable
                && ((ICancellable)window).Cancelled)
                return;

            SetFilter();
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
                    tn.IsExpanded = true;

                    var query = from bk in Workspace.Current.DB.Books
                                where bk.DefaultTopicID == t.ID
                                orderby bk.Title ascending
                                select bk;
                    //For testing. actually need to generate the list from all notes
                    foreach (Book b in query)
                    {
                        tn.Add(new TreeNode(TreeNode.NodeType.Leaf,b, b.Title));
                    }

                    Tree.Add(tn);
                }
            }

            RaisePropertyChanged("Tree");
        }
    }
}
