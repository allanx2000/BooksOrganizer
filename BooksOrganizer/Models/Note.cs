using System;
using System.ComponentModel.DataAnnotations;

namespace BooksOrganizer.Models
{
    public class Note : INodeData
    {
        [Key]
        public int ID { get; set; }

        public Topic Topic { get; set; }
        public SubTopic SubTopic { get; set; }

        [StringLength(50)]
        public string Location { get; set; }
        public string OriginalText { get; set; }
        public string WriteUp { get; set; }
        public string Notes { get; set; }
        public bool? Published { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

    }
}