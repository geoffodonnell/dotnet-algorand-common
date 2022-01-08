using Newtonsoft.Json;

namespace Algorand.Common.Asc {

	[JsonConverter(typeof(ContractConverter))]
	public abstract class Contract {

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

	}

}
