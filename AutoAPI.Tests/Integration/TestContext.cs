﻿using AutoAPI.API;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AutoAPI.Tests.Integration
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
