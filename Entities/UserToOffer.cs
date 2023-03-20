using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JobBoard.Entities
{
    public class UserToOffer
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OfferId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        
        [ForeignKey(nameof(OfferId))]
        public virtual Offer Offer { get; set; }
    }
}
