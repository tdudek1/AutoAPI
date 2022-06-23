using Microsoft.EntityFrameworkCore;

namespace AutoAPI.Web.Entity
{
	[Keyless]
	public class BookView
	{
		public string Author { get; set; }
		public string Book { get; set; }
	}
}
