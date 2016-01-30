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
        Window window;
        public TopicsManagerViewModel(Window window)
        {
            this.window = window;
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
