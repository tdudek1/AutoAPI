using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AutoAPI.Web.Entity
{
    public class Author
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid? UniqueId { get; set; }
        [Required]
        public String Name { get; set; }
        public List<Book> Books { get; set; }
        public DateTime DateOfBirth { get; set; }
        

    }
}
