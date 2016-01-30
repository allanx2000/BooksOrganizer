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
    public class EditNoteViewModel : ViewModel45, ICancellable
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
        private readonly Book book;
        private Note note;
        
        public EditNoteViewModel(Window window, Book book, Note noteForEdit = null)
        {
            this.window = window;
            
            this.book = book;
            this.note = noteForEdit;

            InitializeValues();
        }

        private void InitializeValues()
        {
            if (note == null)
            {
                if (book.DefaultTopic != null)
                    SelectedTopic = book.DefaultTopic;

                return;
            }

            Location = note.Location;
            OriginalText = note.OriginalText;
            IsPublished = note.Published;
            SelectedTopic = note.Topic;
            SelectedSubTopic = note.SubTopic;
            WriteUp = note.WriteUp;
            Notes = note.Notes;
        }

        #region Static Properties
        public string BookTitle
        {
            get { return book.Title; }
        }

        private bool IsEdit
        {
            get
            {
                return note != null;
            }
        }

        public string WindowTitle
        {
            get
            {
                return (IsEdit ? "Edit" : "Add") + " Note";
            }
        }

        public string SaveText
        {
            get
            {
                return IsEdit ? "Edit" : "Add";
            }
        }

        #endregion

        #region Commands

        public ICommand SaveCommand
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
                if (string.IsNullOrEmpty(Location))
                    throw new RequiredFieldException("Location");
                else if (string.IsNullOrEmpty(OriginalText))
                    throw new RequiredFieldException("OriginalText");
                else if (SelectedTopic == null)
                    throw new RequiredFieldException("Topic");
                
                if (IsEdit)
                {
                    note.Location = Location;
                    note.OriginalText = OriginalText;
                    note.Published = IsPublished;
                    note.SubTopic = SelectedSubTopic;
                    note.Topic = SelectedTopic;
                    note.Updated = DateTime.Now;
                    note.WriteUp = WriteUp;
                    note.Notes = Notes;
                }
                else
                {
                    Note note = new Note()
                    {
                        Book = book,
                        Topic = SelectedTopic,
                        Location = Location,
                        OriginalText = OriginalText,
                        SubTopic = SelectedSubTopic,
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        WriteUp = WriteUp,
                        Notes = Notes
                    };
                    
                    Workspace.Current.DB.Notes.Add(note);
                }
                
                Workspace.Current.DB.SaveChanges();
                cancelled = false;

                window.Close();
            }
            catch (Exception e)
            {
                Workspace.Current.DB.RejectChanges();
                MessageBoxFactory.ShowError(e);
            }
        }

        public ICommand CancelCommand
        {
            get { return new CommandHelper(() => window.Close());  }
        }

        #endregion

        #region Fields

        private string notes;
        public string Notes
        {
            get { return notes; }
            set
            {
                notes = value;
                RaisePropertyChanged();
            }
        }

        private string location;
        public string Location
        {
            get { return location; }
            set
            {
                location = value;
                RaisePropertyChanged();
            }
        }

        private string originalText;
        public string OriginalText
        {
            get { return originalText; }
            set
            {
                originalText = value;
                RaisePropertyChanged();
            }
        }

        private bool isPublished;
        public bool IsPublished
        {
            get { return isPublished; }
            set
            {
                isPublished = value;
                RaisePropertyChanged();
            }

        }

        private string writeUp;
        public string WriteUp
        {
            get { return writeUp; }
            set
            {
                writeUp = value;
                RaisePropertyChanged();
            }
        }

        private Topic selectedTopic;
        public Topic SelectedTopic
        {
            get { return selectedTopic; }
            set
            {
                if (value == selectedTopic)
                    return;

                selectedTopic = value;
                RaisePropertyChanged();
                
                SelectedSubTopic = null;

                SubTopics.Clear();
                foreach (SubTopic st in Workspace.Current.GetAllSubTopics(selectedTopic))
                {
                    SubTopics.Add(st);
                }
            }
        }

        
        private SubTopic selectedSubTopic;
        public SubTopic SelectedSubTopic
        {
            get
            {
                return selectedSubTopic;
            }
            set
            {
                selectedSubTopic = value;
                RaisePropertyChanged();
            }
        }
        
        public ICollection<Topic> Topics
        {
            get
            {
                return Workspace.Current.GetAllTopics();
            }
        }

        public ObservableCollection<SubTopic> SubTopics = new ObservableCollection<SubTopic>();

        #endregion

    }
}
