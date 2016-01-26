using Innouvous.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksOrganizer.ViewModels
{
    public class WorkspaceViewModel : ViewModel
    {

        public enum GroupBy
        {
            Title,
            Topic
        }

        public enum OrderBy
        {
            Location,
            Created,
            Updated
        }

        private GroupBy selectedGroupBy;
        public GroupBy SelectedGroupBy
        {
            get
            {
                return selectedGroupBy;
            }
            set
            {
                selectedGroupBy = value;
                RaisePropertyChanged("SelectedGroupBy");
                RaisePropertyChanged("ByTitle");
                RaisePropertyChanged("ByTopic");

            }
        }
        public bool ByTitle
        {
            get
            {
                return selectedGroupBy == GroupBy.Title;
            }
            set
            {
                SelectedGroupBy = GroupBy.Title;
            }
        }
        public bool ByTopic
        {
            get
            {
                return selectedGroupBy == GroupBy.Topic;
            }
            set
            {
                SelectedGroupBy = GroupBy.Topic;
            }
        }
        
        private OrderBy selectedOrderBy;

        private WorkspaceWindow window;

        public WorkspaceViewModel(WorkspaceWindow window)
        {
            this.window = window;
        }

        public OrderBy SelectedOrderBy
        {
            get;
        }

    }
}
