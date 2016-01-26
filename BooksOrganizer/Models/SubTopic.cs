using System.ComponentModel.DataAnnotations;

namespace BooksOrganizer.Models
{
    public class SubTopic
    {
        [Key]
        public int ID { get; set; }

        public Topic ParentTopic { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
    }
}