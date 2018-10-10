using AutoAPI.Web.Entity;
using Microsoft.EntityFrameworkCore;
using System;

namespace AutoAPI.Web
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        [AutoAPIEntity(Route = "authors", ExposePagedResult = true)]
        public DbSet<Author> Authors { get; set; }
        [AutoAPIEntity(Route = "Books", Authorize = true, POSTPolicy = "IsAdmin")]
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().HasData(new Author[] {
                new Author()
                {
                    Id = 1,
                    Name = "Ernest Hemingway",
                    DateOfBirth = new DateTime(1899, 7, 21)
                },
               new Author()
                {
                    Id = 2,
                    Name = "Stephen King",
                    DateOfBirth = new DateTime(1947, 9, 21)
                }
            });


            modelBuilder.Entity<Book>().HasData(new Book[] {
                new Book() { AuthorId = 1, ISBN = "1234 5", Title = "For Whom the Bell Tolls" },
                new Book() { AuthorId = 1, ISBN = "5678", Title = "A Farewell to Arms" }
            });

            modelBuilder.Entity<Book>().HasData(new Book[] {
                new Book() { AuthorId = 2, ISBN = "99999", Title = "IT" },
                new Book() { AuthorId = 2, ISBN = "324423423", Title = "The Shining" }
            });
        }

    }

}

