using BooksOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksOrganizer
{
    class Util
    {
        public static WorkspaceContext DB
        {
            get
            {
                return Workspace.Current.DB;
            }
        }
        
    }
}
