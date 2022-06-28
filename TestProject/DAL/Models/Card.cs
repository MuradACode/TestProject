using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class Card : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

        public string ImageUrl { get; set; }
    }
}
