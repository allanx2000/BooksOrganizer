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
                ((CommandHelper)addNoteCommand).RaiseCanExecuteChanged();

                RaisePropertyChanged();
            }
        }



        private WorkspaceWindow window;

        public WorkspaceViewModel(WorkspaceWindow window)
        {
            this.window = window;

            editCommand = new CommandHelper(Edit, (o) => selectedNode != null);
            addNoteCommand = new CommandHelper(AddNote, (o) => selectedNode != null && (selectedNode.IsNote || selectedNode.IsBook));

            Tree = new ObservableCollection<TreeNode>();
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

                INodeData parent = null;

                if (SelectedNode.IsBook)
                {
                    Book bk = (Book)SelectedNode.GetData();
                    
                    if (!MessageBoxFactory.ShowConfirmAsBool("Delete book and all notes for '" + bk.Title + "'?", "Delete Book"))
                        return;

                    parent = bk.DefaultTopic;
                    Util.DB.Books.Remove(bk);
                    
                }
                else if (SelectedNode.IsNote)
                {
                    Note n = (Note)SelectedNode.GetData();
                    if (!MessageBoxFactory.ShowConfirmAsBool("Delete " + n.OriginalText + "'?", "Delete Note"))
                        return;

                    parent = n.Book;
                    Util.DB.Notes.Remove(n);
                    
                }
                else if (SelectedNode.IsTopic)
                    Util.DB.Topics.Remove((Topic)SelectedNode.GetData());
                else
                    throw new Exception("Type not supported: " + SelectedNode.GetDataType());

                Util.DB.SaveChanges();
                SetFilter(parent);
            }
            catch (Exception e)
            {
                Util.DB.RejectChanges();
                MessageBoxFactory.ShowError(e);
            }
        }

        

            public ICommand BulkAddNotesCommand
        {
            get
            {
                return new CommandHelper(BulkAddNotes);
            }
        }

        public void BulkAddNotes()
        {
            Book bk = null;
            
            if (SelectedNode != null)
            {
                if (SelectedNode.IsNote)
                    bk = ((Note)selectedNode.GetData()).Book;
                else if (selectedNode.IsBook)
                    bk = ((Book)selectedNode.GetData());
            }

            var window = new BulkAddNotesWindow(bk);
            window.ShowDialog();
            
            SetFilter(window);
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

            SetFilter(window);
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

            SetFilter(window);


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

            Window window = null;

            if (selectedNode.IsBook)
            {
                window = new EditBookWindow((Book)selectedNode.GetData());
                window.ShowDialog();

            }
            else if (selectedNode.IsNote)
            {
                Note n = (Note)selectedNode.GetData();
                window = new EditNotesWindow(n.Book, n);
                window.ShowDialog();
            }
            
            SetFilter(window);
        }

        private readonly ICommand addNoteCommand;
        public ICommand AddNoteCommand
        {
            get
            {
                return addNoteCommand;
            }
        }

        private void AddNote()
        {
            Book bk;

            if (SelectedNode == null)
                return;
            else if (SelectedNode.IsNote)
                bk = ((Note)selectedNode.GetData()).Book;
            else if (selectedNode.IsBook)
                bk = ((Book)selectedNode.GetData());
            else
                return;

            var window = new EditNotesWindow(bk);

            window.ShowDialog();

            SetFilter(window);
        }


        /*
        private readonly ICommand ctxBulkAddNotesCommand;
        public ICommand CtxBulkAddNotesCommand
        {
            get
            {
                return ctxBulkAddNotesCommand;
            }
        }

        private void CtxBulkAddNotes()
        {
            Book bk;

            if (SelectedNode == null)
                return;
            else if (SelectedNode.IsNote)
                bk = ((Note)selectedNode.GetData()).Book;
            else if (selectedNode.IsBook)
                bk = ((Book)selectedNode.GetData());
            else
                return;

            var window = new BulkAddNotesWindow(bk);

            window.ShowDialog();

            SetFilter(window);
        }
        */

        public ICommand RefreshCommand
        {
            get
            {
                return new CommandHelper(() => SetFilter());
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

            SetFilter(SelectedNode == null? null : SelectedNode.GetData());
        }

        private void SetFilter(INodeData selected = null)
        {
            Tree.Clear();

            Dictionary<int, List<Note>> notes = Workspace.Current.GetAllNotesGrouped();

            if (SelectedGroupBy == GroupBy.Title)
            {
                foreach (Book b in Workspace.Current.GetAllBooks())
                {
                    TreeNode bookNode = MakeBook(notes, b);
                    SelectAndExpand(selected, bookNode);

                    Tree.Add(bookNode);
                }
            }
            else if (SelectedGroupBy == GroupBy.Topic)
            {
                foreach (Topic t in Workspace.Current.DB.Topics.OrderBy(x => x.Name))
                {
                    var tn = new TreeNode(TreeNode.NodeType.Node, t, t.Name);
                    SelectAndExpand(selected, tn);

                    var query = from bk in Workspace.Current.DB.Books
                                where bk.DefaultTopicID == t.ID
                                orderby bk.Title ascending
                                select bk;
                    
                    //For testing. actually need to generate the list from all notes
                    foreach (Book b in query)
                    {

                        var bk = MakeBook(notes, b);
                        tn.Add(bk);

                        SelectAndExpand(selected, bk);
                    }

                    Tree.Add(tn);
                }
            }

            RaisePropertyChanged("Tree");
        }

        private void SelectAndExpand(INodeData toMatch, TreeNode node)
        {
            if (node.GetData() == toMatch)
            {
                TreeNode n = node;
                do
                {
                    n.IsExpanded = true;
                    n = n.Parent;
                }
                while (n != null);

                SelectedNode = node;
                
            }
        }

        private static TreeNode MakeBook(Dictionary<int, List<Note>> notes, Book b)
        {
            var bookNode = new TreeNode(TreeNode.NodeType.Node, b, b.Title);

            if (notes.ContainsKey(b.ID))
            {
                foreach (Note n in notes[b.ID])
                    bookNode.Add(new TreeNode(TreeNode.NodeType.Leaf, n, n.Location + ": " + n.OriginalText));
            }

            return bookNode;
        }
    }
}
