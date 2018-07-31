using AutoAPI.API;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AutoAPI.Tests
{
    public class TestContext : DbContext
    {

        public TestContext(DbContextOptions<TestContext> options) : base(options) { }

        [AutoAPIEntity(Route = "authors")]
        public DbSet<Author> Authors { get; set; }
        [AutoAPIEntity(Route = "Books")]
        public DbSet<Book> Books { get; set; }
    }

    public class Author
    {
        [Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
        public String Name { get; set; }
        public List<Book> Books { get; set; }

    }

    public class Book
    {
        [Key]
        public string ISBN { get; set; }
        public string Title { get; set; }
        public int AuthorId { get; set; }
    }
}