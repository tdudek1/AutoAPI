using AutoAPI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoAPI.Tests
{
    public static class SetupHelper
    {

        public static TestContext BuildTestContext()
        {
            var options = new DbContextOptionsBuilder<TestContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            var context = new TestContext(options);

            context.Add(new Author()
            {
                Id = 1,
                Name = "Ernest Hemingway",
                Books = new List<Book>() {
                    new Book() { ISBN = "1234 5", Title ="For Whom the Bell Tolls" }, new Book() { ISBN = "5678", Title = "A Farewell to Arms" } }
            });

            context.Add(new Author()
            {
                Id = 2,
                Name = "Stephen King",
                Books = new List<Book>() {
                    new Book() { ISBN = "99999", Title ="IT" }, new Book() { ISBN = "324423423", Title = "The Shining" } }
            });

            context.SaveChanges();
            context.Dispose();

            return new TestContext(options);
        }

    }
}