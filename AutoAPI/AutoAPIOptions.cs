using Newtonsoft.Json;
using System.Text.Json;

namespace AutoAPI
{
	public class AutoAPIOptions
	{
		public string Path { get; set; }

		public JsonSerializerOptions JsonSerializerOptions { get; set; }

		public JsonSerializerSettings JsonSerializerSettings { get; set; }

		public bool UseNewtonoftSerializer { get; set; }

	}
}
