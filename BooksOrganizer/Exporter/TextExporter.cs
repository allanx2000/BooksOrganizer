using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksOrganizer.Models;
using System.IO;

namespace BooksOrganizer.Exporter
{
    class TextExporter : BaseExporter
    {
        public override void Export(Book book, string path)
        {
            StreamWriter sw = new StreamWriter(path);

            StringBuilder sb = new StringBuilder();

            foreach (var note in book.Notes)
            {
                if (IncludeLocation)
                    sb.Append(note.Location + ": ");

                sb.Append(note.OriginalText);

                sb.AppendLine();

                sw.WriteLine(sb.ToString());
                sb.Clear();
            }

            sw.Close();
        }
    }
}
