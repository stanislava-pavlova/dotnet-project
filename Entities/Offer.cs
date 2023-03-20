using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JobBoard.Entities
{
    public class Offer
    {
        [Key]
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public virtual User Owner { get; set; }

    }
}
