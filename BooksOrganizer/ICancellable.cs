using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksOrganizer
{
    interface ICancellable
    {
        bool Cancelled { get; }
    }
}
