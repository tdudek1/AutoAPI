using AutoAPI;
using AutoAPI.Web;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoAPI.Tests
{
    public static class SetupHelper
    {

        public static DataContext BuildTestContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            var context = new DataContext(options);

            context.Database.EnsureCreated();

            return new DataContext(options);
        }

    }
}