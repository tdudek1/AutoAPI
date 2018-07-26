using AutoAPI.API;
using AutoAPI.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoAPI
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        [AutoAPIEntity(Route = "persons")]
        public DbSet<Person> Persons { get; set; }
        [AutoAPIEntity(Route = "addresses")]
        public DbSet<Address> Addresses { get; set; }


    }
}
