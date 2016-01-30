using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksOrganizer.Models
{
    public class SubTopic : INodeData
    {
        [Key]
        public int ID { get; set; }

        [Column("ParentTopic")]
        public virtual int ParentTopicId { get; set; }
        public virtual Topic ParentTopic { get; set; }

        [StringLength(50)]
        public virtual string Name { get; set; }
    }
}