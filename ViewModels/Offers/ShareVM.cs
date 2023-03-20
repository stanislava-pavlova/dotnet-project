using JobBoard.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobBoard.ViewModels.Offers
{
    public class ShareVM
    {
        public int OfferId { get; set; }
        public List<int> UserIds { get; set; }

        public Offer Offer { get; set; }
        public List<User> Users { get; set; }

        public List<UserToOffer> Shares { get; set; }

    }
}
