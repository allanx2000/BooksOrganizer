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
    /// Interaction logic for WorkspaceWindow.xaml
    /// </summary>
    public partial class WorkspaceWindow : Window
    {
        private readonly WorkspaceViewModel vm;

        public WorkspaceWindow()
        {
            vm = new WorkspaceViewModel(this);

            this.DataContext = vm;

            InitializeComponent();

            //TODO: Need to do a query when load db, in loader
            /*
            var db = Workspace.Current.DB;

            db.Books.Add(new Models.Book() {
                Created = DateTime.Now, //Move to constructor
                Title = "Test"
            });

            db.SaveChanges();
            */
        }
        
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            vm.SelectedNode = e.NewValue;
        }
    }
}
