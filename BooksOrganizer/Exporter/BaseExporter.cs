using BooksOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksOrganizer.Exporter
{
    public abstract class BaseExporter
    {
        public abstract void Export(Book book, string path);
        public bool IncludeLocation { get; set; }
    }
}
