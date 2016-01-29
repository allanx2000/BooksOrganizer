using Innouvous.Utils;
using Innouvous.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Settings = BooksOrganizer.Properties.Settings;

namespace BooksOrganizer.ViewModels
{
    public class LoaderViewModel : ViewModel45
    {
        private Loader window;
        
        public LoaderViewModel(Loader window)
        {
            this.window = window;

            if (Settings.Default.PathHistory == null)
            {
                List<string> items = new List<string>();

                Settings.Default.PathHistory = new System.Collections.Specialized.StringCollection();
                Settings.Default.Save();

                PathHistory = items;
            }
            else
                PathHistory = Settings.Default.PathHistory.Cast<string>().ToList();
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
                RaisePropertyChanged();
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

                if (!Settings.Default.PathHistory.Contains(selectedPath))
                {
                    Settings.Default.PathHistory.Add(selectedPath);
                    Settings.Default.Save();
                }

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
