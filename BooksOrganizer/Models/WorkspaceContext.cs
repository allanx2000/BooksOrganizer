using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksOrganizer.Models
{
    /// <summary>
    /// Defines the context for Entity Framework
    /// </summary>
    public class WorkspaceContext : DbContext
    {
        public static WorkspaceContext GetContext(string dbPath)
        {
            string connString = "Data Source={0}";
            connString = String.Format(connString, dbPath);

            return new WorkspaceContext(connString);
        }
        

        public DbSet<Note> Notes { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Topic> Topics { get; set; }
        //public DbSet<SubTopic> SubTopics { get; set; }
        
        private WorkspaceContext(string connectionString) : base(new SQLiteConnection(connectionString), true)
        {
        }

        public void RejectChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        {
                            entry.CurrentValues.SetValues(entry.OriginalValues);
                            entry.State = EntityState.Unchanged;
                            break;
                        }
                    case EntityState.Deleted:
                        {
                            entry.State = EntityState.Unchanged;
                            break;
                        }
                    case EntityState.Added:
                        {
                            entry.State = EntityState.Detached;
                            break;
                        }
                }
            }
        }

    }
}
