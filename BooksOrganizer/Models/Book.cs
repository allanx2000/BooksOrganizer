using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksOrganizer.Models
{
    public class Book
    {
        #region "Keys"
        //This relationship needs to be enforce manually... not sure how to set

        [Key]
        public int ID { get; set; }

        public DateTime Created { get; set; }
        
        public int? DefaultTopicID { get; set; }

        public virtual Topic DefaultTopic { get; set; }
        #endregion

        [Column("Name")]
        public string Title { get; set; }
        
        public int? Rating { get; set; }

        public string Comments { get; set; }
        
        public virtual ICollection<Note> Notes { get; set; }
        
        /// <summary>
        /// Gets all topics in the notes
        /// </summary>
        /// <returns></returns>
        public List<Topic> GetTopics()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return Title.ToLower().GetHashCode();
        }

        //TODO: Implement?
        public void AddNote(Note note)
        {
            throw new NotImplementedException();
        }

        public void RemoveNote(Note note)
        {
            throw new NotImplementedException();
        }
        
    }
}