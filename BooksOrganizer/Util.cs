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

        internal static bool ToBool(bool? nullable, bool def = false)
        {
            if (nullable == null)
                return def;
            else return nullable.Value;
        }

        internal static bool? ToNullableBool(bool b, bool nullValue = false)
        {
            if (b == nullValue)
                return null;
            else return b;
        }
    }
}
