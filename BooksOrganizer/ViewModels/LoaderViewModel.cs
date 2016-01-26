using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BooksOrganizer.ViewModels
{
    public class LoaderViewModel : ViewModel
    {
        private Loader window;
        public LoaderViewModel(Loader window)
        {
            this.window = window;
        }

        public List<string> PathHistory
        {
            get;
            private set;
        }

        private string selectedPath;
        public string SelectedPath
        {
            get
            {
                return selectedPath;
            }
            set
            {
                selectedPath = value;
                RaisePropertyChanged("SelectedPath");
            }
        }

        public ICommand ExploreCommand
        {
            get { return new CommandHelper(Explore); }
        }

        private void Explore()
        {

        }

        public ICommand LoadCommand
        {
            get { return new CommandHelper(Load); }
        }

        public void Load()
        {
            try
            {
                FileInfo fi = new FileInfo(SelectedPath);

                if (!fi.Exists)
                {
                    if (MessageBoxFactory.ShowConfirmAsBool("Do you want to create a new workspace?", "Create New Workspace"))
                    {
                        Workspace.CreateWorkspace(SelectedPath);
                    }
                    else 
                        return;
                }
                else
                    Workspace.LoadWorkspace(SelectedPath);

                WorkspaceWindow wiw = new WorkspaceWindow();
                wiw.Show();
                window.Close();
            }
            catch (Exception e)
            {
                MessageBoxFactory.ShowError(e, "Error Loading Workspace");
            }
        }

    }
}
