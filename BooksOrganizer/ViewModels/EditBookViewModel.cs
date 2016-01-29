using BooksOrganizer.Models;
using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BooksOrganizer.ViewModels
{
    public class EditBookViewModel : ViewModel45
    {
        private EditBookWindow window;

        private Book book;

        public EditBookViewModel(EditBookWindow window, Book bookForEdit = null)
        {
            Ratings = new List<string>();
            Ratings.Add(NotRead);

            for (int i = 0; i <= 5; i++)
            {
                Ratings.Add(i.ToString());
            }

            this.window = window;
            book = bookForEdit;

            InitializeValues();
        }

        private bool IsEdit
        {
            get
            {
                return book != null;
            }
        }

        public string WindowTitle
        {
            get
            {
                return (IsEdit ? "Edit" : "Add") + " Book";
            }
        }

        public string SaveChangeText
        {
            get
            {
                return IsEdit ? "Edit" : "Add";
            }
        }

        public ICommand SaveChangeCommand
        {
            get
            {
                return new CommandHelper(SaveChange);
            }
        }

        private void SaveChange()
        {
            try
            {
                if (IsEdit)
                {
                }
                else
                {
                    Book bk = new Book()
                    {
                        DefaultTopic = DefaultTopic,
                        Created = DateTime.Now,
                        Rating = GetDbRating(),
                        Title = Title,
                        Comments = Comments
                    };
                    
                    Workspace.Current.DB.Books.Add(bk);
                    Workspace.Current.DB.SaveChanges();
                }

                window.Close();
            }
            catch (Exception e)
            {
                Workspace.Current.DB.RejectChanges();
                MessageBoxFactory.ShowError(e);
            }
        }

        private int? GetDbRating()
        {
            if (SelectedRating == NotRead)
                return null;
            else
                return Convert.ToInt32(SelectedRating);
        }

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged();
            }
        }

        private string comments;
        public string Comments
        {
            get { return comments; }
            set
            {
                comments = value;
                RaisePropertyChanged();
            }
        }
        
        private Topic defaultTopic;
        public Topic DefaultTopic
        {
            get { return defaultTopic; }
            set
            {
                defaultTopic = value;
                RaisePropertyChanged();
            }
        }

        private const string NotRead = "Not Read";

        private string selectedRating;
        public string SelectedRating
        {
            get
            {
                return selectedRating;
            }
            set
            {
                selectedRating = value;
                RaisePropertyChanged("SelectedRating");
            }
        }
        public List<string> Ratings
        {
            get; private set;
        }


        public ICollection<Topic> Topics
        {
            get
            {
                return Workspace.Current.DB.Topics.OrderBy(x => x.Name).ToList();
            }
        }

        private void InitializeValues()
        {
            if (book == null)
                return;

            Title = book.Title;
            DefaultTopic = book.DefaultTopic;
        }
    }
}
