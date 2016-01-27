﻿using System.ComponentModel.DataAnnotations;

namespace BooksOrganizer.Models
{
    public class Topic : INodeData
    {
        [Key]
        public int ID { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
    }
}