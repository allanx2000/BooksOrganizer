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

namespace BooksOrganizer
{
    /// <summary>
    /// Interaction logic for BulkAddNotesWindow.xaml
    /// </summary>
    public partial class BulkAddNotesWindow : Window
    {
        private BulkAddNotesViewModel vm;
        public BulkAddNotesWindow()
        {
            vm = new BulkAddNotesViewModel(this);
            this.DataContext = vm;

            InitializeComponent();
        }
    }
}
