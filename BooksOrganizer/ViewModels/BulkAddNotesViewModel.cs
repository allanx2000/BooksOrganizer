using BooksOrganizer.Models;
using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BooksOrganizer.ViewModels
{
    public class BulkAddNotesViewModel : ViewModel45, ICancellable
    {
        private bool cancelled = true;
        public bool Cancelled
        {
            get
            {
                return cancelled;
            }
        }


        private readonly Window window;

        public BulkAddNotesViewModel(Window window)
        {
            this.window = window;

            ParsedNotes = new ObservableCollection<TreeNode>();
        }

        #region Properties

        private string inputText;
        public string InputText
        {
            get { return inputText; }
            set
            {
                inputText = value;
                RaisePropertyChanged();
                RaisePropertyChanged("CanParse");
            }
        }

        public bool CanParse { get { return !string.IsNullOrEmpty(inputText); } }

        private Book selectedBook;
        public Book SelectedBook { get { return selectedBook; }
            set
            {
                selectedBook = value;
                RaisePropertyChanged();
            }
        }

        public ICollection<Book> Books
        {
            get
            {
                return Workspace.Current.GetAllBooks();
            }
        }

        //TODO: Change better
        public ObservableCollection<TreeNode> ParsedNotes { get; private set; }

        

                    public ICommand AddCommand
        {
            get
            {
                return new CommandHelper(AddAll);
            }
        }

        private void AddAll()
        {
            try
            {
                foreach (TreeNode n in ParsedNotes)
                {
                    Util.DB.Notes.Add((Note)n.GetData());
                }

                Util.DB.SaveChanges();
                this.cancelled = false;
                window.Close();
            }
            catch (Exception e)
            {
                Util.DB.RejectChanges();
                MessageBoxFactory.ShowError(e);
            }
        }


        public ICommand ParseCommand
        {
            get
            {
                return new CommandHelper(Parse);
            }
        }

        private void Parse()
        {
            try
            {
                ParsedNotes.Clear();

                StringReader sr = new StringReader(InputText);

                string line;

                string current = "";
                string NoteEnd = "Add a note";
                string LocationStart = "Read more at location ";
                while ((line = sr.ReadLine()) != null)
                {
                    current += line + " ";

                    if (current.Contains(NoteEnd))
                    {
                        int start, end;

                        start = current.IndexOf(LocationStart);
                        string text = current.Substring(0, start);

                        start = start + LocationStart.Length;
                        end = current.IndexOf(" ", start + 1);

                        string location = current.Substring(start, end - start);

                        Note note = new Note()
                        {
                            Created = DateTime.Now,
                            Updated = DateTime.Now,
                            Book = SelectedBook,
                            Location = location,
                            OriginalText = text,
                            Topic = SelectedBook.DefaultTopic
                        };

                        ParsedNotes.Add(new TreeNode(TreeNode.NodeType.Leaf, note, note.OriginalText));

                        current = "";
                    }
                }

            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        #endregion

    }
}
