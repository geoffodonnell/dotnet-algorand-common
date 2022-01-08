using Newtonsoft.Json;

namespace Algorand.Common.Asc {

	public class Variable {

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("index")]
		public int Index { get; set; }

		[JsonProperty("length")]
		public int Length { get; set; }

	}

}
