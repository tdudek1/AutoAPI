using AutoAPI.Web.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoAPI.Web
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


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Person>().HasData(
				new Person
				{
					Id = 1,
					FirstName = "William",
					LastName = "Shakespeare"
				},
				new Person()
				{
					Id = 2,
					FirstName = "John",
					LastName = "Adams"
				}
			);


			modelBuilder.Entity<Address>().HasData(
				new Address()
				{
					Id = 2,
					Street = "123 Maple Ave",
					City = "New York",
					State = "NY",
					Zip = "10001",
					PersonId = 1
				},
				new Address
				{
					Id = 1,
					Street = "123 Main ST",
					City = "Chicago",
					State = "IL",
					Zip = "60606",
					PersonId = 2
				});
		}

	}
}
