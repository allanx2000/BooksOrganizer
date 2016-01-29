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
    /// Interaction logic for TopicsManager.xaml
    /// </summary>
    public partial class TopicsManager : Window
    {
        private readonly TopicsManagerViewModel vm;
        public TopicsManager()
        {
            InitializeComponent();

            vm = new TopicsManagerViewModel(this);
            this.DataContext = vm;
        }
    }
}
