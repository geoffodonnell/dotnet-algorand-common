using Newtonsoft.Json;

namespace Algorand.Common.Asc {

	public class Schema {

		[JsonProperty("num_uints")]
		public int NumUints { get; set; }

		[JsonProperty("num_byte_slices")]
		public int NumByteSlices { get; set; }

	}

}
