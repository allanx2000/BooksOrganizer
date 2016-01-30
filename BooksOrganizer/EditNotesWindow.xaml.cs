using BooksOrganizer.ViewModels;
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
using BooksOrganizer.Models;

namespace BooksOrganizer
{
    /// <summary>
    /// Interaction logic for EditBookWindow.xaml
    /// </summary>
    public partial class EditNotesWindow : Window, ICancellable
    {
        private EditNoteViewModel vm;
        public bool Cancelled
        {
            get
            {
                return vm.Cancelled;
            }
        }

        public EditNotesWindow(Book book)
        {
            Load(book);
        }

        public EditNotesWindow(Book book, Note note)
        {
            Load(book, note);
        }
        
        private void Load(Book book, Note note = null)
        {
            InitializeComponent();

            vm = new EditNoteViewModel(this, book, note);
            this.DataContext = vm;
        }

    }
}
