using System;
using System.ComponentModel.DataAnnotations;

namespace AutoAPI.Web.Entity
{
	public class Genre
	{
		[Key]
		public Guid Id { get; set; }

		public string Name { get; set; }
	}
}
