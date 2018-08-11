using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AutoAPI.Web.Entity
{

    public class Book
    {
        [Key]
        public string ISBN { get; set; }
        public string Title { get; set; }
        public int AuthorId { get; set; }
    }

}
