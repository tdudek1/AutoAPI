using System.Text.Json;

namespace AutoAPI
{
	public class AutoAPIOptions
	{
		public string Path { get; set; }

		public JsonSerializerOptions JsonSerializerOptions { get; set; }

	}
}
