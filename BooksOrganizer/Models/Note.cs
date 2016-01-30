using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksOrganizer.Models
{
    public class Note : INodeData
    {
        [Key]
        public int ID { get; set; }

        [Column("Topic")]
        public int TopicId { get; set; }
        public virtual Topic Topic { get; set; }

        [Column("Book")]
        public int BookId { get; set; }
        public virtual Book Book { get; set; }

        [Column("SubTopic")]
        public int SubTopicId { get; set; }
        public virtual SubTopic SubTopic { get; set; }

        [StringLength(50)]
        public string Location { get; set; }
        public string OriginalText { get; set; }
        public string WriteUp { get; set; }
        public string Notes { get; set; }
        public bool Published { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

    }
}