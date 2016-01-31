using BooksOrganizer.Models;
using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace BooksOrganizer
{
    /// <summary>
    /// Interaction logic for EditTopicWindow.xaml
    /// </summary>
    public partial class EditTopicWindow : Window
    {
        
        private readonly EditTopicViewModel vm;

        public EditTopicWindow() : this(null)
        {
        }

        public EditTopicWindow(Topic topic)
        {
            InitializeComponent();

            vm = new EditTopicViewModel(this, topic);
        }
    }

    public enum TopicType
    {
        Topic,
        Subtopic
    }


    public class EditTopicViewModel : ViewModel
    {
        private Window window;
        private Topic existing;
        
        public EditTopicViewModel(Window window, Topic topic = null)
        {
            this.window = window;
            existing = topic;
        }
        
        private string name;
        public string Name {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged();
            }
        }

        public bool IsEdit
        {
            get { return existing != null; }
        }

        public ICommand SaveCommand
        {
            get { return new CommandHelper(Save); }
        }

        private void Save()
        {
            try
            {
                if (String.IsNullOrEmpty(Name))
                    throw new Exception("Name is empty");

                if (IsEdit)
                {
                    existing.Name = Name;
                }
                else
                {
                    Topic t = new Topic()
                    {
                        Name = Name
                    };

                    Util.DB.Topics.Add(t);
                }

                Util.DB.SaveChanges();
            }
            catch (Exception e)
            {
                Util.DB.RejectChanges();
                MessageBoxFactory.ShowError(e);
            }
        }

        public ICommand CancelCommand
        {
            get { return new CommandHelper(() => window.Close()); }
        }


    }
}
