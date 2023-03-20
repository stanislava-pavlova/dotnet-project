using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JobBoard.ViewModels.Offers
{
    public class CreateVM
    {
        public int OwnerId { get; set; }

        [DisplayName("Title: ")]
        [Required(ErrorMessage="*This field is Required!")]
        public string Title { get; set; }

        [DisplayName("Description: ")]
        [Required(ErrorMessage = "*This field is Required!")]
        public string Description { get; set; }

        [DisplayName("Category: ")]
        [Required(ErrorMessage = "*This field is Required!")]
        public string Category { get; set; }
    }
}
