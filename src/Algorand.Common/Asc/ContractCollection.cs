using Newtonsoft.Json;
using System.Collections.Generic;

namespace Algorand.Common.Asc {

	public class ContractCollection {

		[JsonProperty("repo")]
		public string Repo { get; set; }
		
		[JsonProperty("ref")]
		public string Reference { get; set; }

		[JsonProperty("contracts")]
		public Dictionary<string, Contract> Contracts { get; set; }

	}

}
