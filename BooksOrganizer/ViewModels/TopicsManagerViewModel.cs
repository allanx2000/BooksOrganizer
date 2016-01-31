using BooksOrganizer.Controls;
using BooksOrganizer.Models;
using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BooksOrganizer.ViewModels
{
    public class TopicsManagerViewModel : ViewModel45
    {
        private readonly Window window;
        private readonly ITree tree;
        public TopicsManagerViewModel(Window window, ITree topicsTree)
        {
            this.window = window;

            topicsTree.SetGenerateTree(MakeTree);
        }

        private ICollection<TreeNode> MakeTree()
        {
            List<TreeNode> tree = new List<TreeNode>();

            foreach (Topic t in Workspace.Current.GetAllTopics())
            {
                var tn = TreeNode.CreateNode(t, t.Name);

                /*
                foreach (SubTopic s in t.SubTopics)
                {
                    tn.Add(TreeNode.CreateLeaf(s, s.Name));
                }*/

                tree.Add(tn);
            }

            return tree;
        }

        public Topic SelectedTopic { get; set; }

        public ICollection<Topic> Topics
        {
            get
            {
                return Workspace.Current.GetAllTopics();
            }
        }

        #region Add New Topic
        private string name;
        public string Name {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged();
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return new CommandHelper(() => window.Close());
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return new CommandHelper(Delete);
            }
        }

        private void Delete()
        {
            try
            {
                if (SelectedTopic != null)
                {
                    var notes = Util.DB.Notes.Count(x => x.TopicId == SelectedTopic.ID);

                    if (MessageBoxFactory.ShowConfirmAsBool(SelectedTopic.Name + " has " + notes + " notes. These will be orphaned. Continue?", "Confirm Delete"))
                    {
                        Util.DB.Topics.Remove(SelectedTopic);
                        Util.DB.SaveChanges();

                        RaisePropertyChanged("Topic");
                    }
                }
            }
            catch (Exception e)
            {
                Util.DB.RejectChanges();
                MessageBoxFactory.ShowError(e);
            }
        }

        public ICommand AddCommand
        {
            get
            {
                return new CommandHelper(Add);
            }
        }


        private void Add()
        {
            try
            {
                Workspace.Current.DB.Topics.Add(new Models.Topic() {
                    Name = this.Name
                });
                
                Workspace.Current.DB.SaveChanges();

                Name = "";
                RaisePropertyChanged("Topics");
            }
            catch (Exception e)
            {
                Workspace.Current.DB.RejectChanges();

                MessageBoxFactory.ShowError(e);
            }
        }

        #endregion
    }
}
