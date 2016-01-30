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
    public partial class EditBookWindow : Window, ICancellable
    {
        private EditBookViewModel vm;
        public bool Cancelled
        {
            get
            {
                return vm.Cancelled;
            }
        }

        public EditBookWindow()
        {
            Load();
        }

        public EditBookWindow(Book book)
        {
            Load(book);
        }
        
        private void Load(Book book = null)
        {
            InitializeComponent();

            vm = new EditBookViewModel(this, book);
            this.DataContext = vm;
        }

    }
}
