using JobBoard.Entities;
using JobBoard.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobBoard.ViewModels.Offers
{
    public class IndexVM
    {
        public List<Offer> Items { get; set; }
        public FilterVM Filter { get; set; }
        public PagerVM Pager { get; set; }
    }
}
