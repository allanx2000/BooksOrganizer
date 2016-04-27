using BooksOrganizer.Exporter;
using BooksOrganizer.Models;
using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BooksOrganizer.ViewModels
{
    public class ExporterViewModel : ViewModel45
    {
        private Window window;

        public ExporterViewModel(Window window)
        {
            this.window = window;

            SelectedExporter = TextExporter;
        }

        public const string TextExporter = "Text";

        private List<string> exporters = new List<string>()
        {
            TextExporter
        };

        public List<Book> Books
        {
            get
            {
                return Workspace.Current.DB.Books.OrderBy(x=>x.Title).ToList();
            }
        }

        private Book selectedBook;
        public Book SelectedBook
        {
            get { return selectedBook; }
            set
            {
                selectedBook = value;

                if (selectedBook != null)
                {
                    string bookName = selectedBook.Title;
                    if (bookName.Length > MaxLength)
                        bookName = bookName.Substring(0, MaxLength);

                    SavePath = Path.Combine(Workspace.Current.Directory, bookName + ".txt");
                }
                else

                    SavePath = "";
                RaisePropertyChanged();
            }
        }
        
        public List<string> Exporters
        {
            get
            {
                return exporters;
            }
        }

        private string selectedExporter;
        public string SelectedExporter
        {
            get
            {
                return selectedExporter;
            }
            set
            {
                selectedExporter = value;
                RaisePropertyChanged();
            }
        }

        private string savePath;
        public string SavePath
        {
            get
            {
                return savePath;
            }
            set
            {
                savePath = value;
                RaisePropertyChanged();
            }
        }

        private bool includeLocation;
        private const int MaxLength = 20;

        public bool IncludeLocation
        {
            get
            {
                return includeLocation;
            }
            set
            {
                includeLocation = value;
                RaisePropertyChanged();
            }
        }

        public ICommand ExportCommand
        {
            get
            {
                return new CommandHelper(Export);
            }
        }

        private void Export()
        {
            try
            {
                if (string.IsNullOrEmpty(SavePath))
                    throw new Exception("Save path not defined.");
                else if (string.IsNullOrEmpty(SelectedExporter))
                    throw new Exception("No exporter selected.");
                else if (SelectedBook == null)
                    throw new Exception("No book selected.");

                if (File.Exists(SavePath)
                    && !MessageBoxFactory.ShowConfirmAsBool("Overwrite existing file?", "File Exists"))
                    return;

                BaseExporter exporter = GetExporter();
                exporter.Export(SelectedBook, SavePath);

                MessageBoxFactory.ShowInfo("Book has been exported to: " + SavePath, "Book Exported");
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e);
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return new CommandHelper(() => window.Close());
            }
        }

        private BaseExporter GetExporter()
        {
            BaseExporter exporter;
            switch (SelectedExporter)
            {
                case TextExporter:
                    exporter = new TextExporter();
                    break;
                default:
                    throw new Exception("Exporter not supported: " + selectedExporter);
            }

            

            return exporter;
        }
    }
}
