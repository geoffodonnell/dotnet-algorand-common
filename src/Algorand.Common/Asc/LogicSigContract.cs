using Newtonsoft.Json;

namespace Algorand.Common.Asc {

	[ContractType(TypeKeyValue)]
	public class LogicSigContract : Contract {

		public const string TypeKeyValue = "logicsig";

		[JsonProperty("logic")]
		public ProgramLogic Logic { get; set; }

		public LogicSigContract() {
			Type = TypeKeyValue;
		}

	}

}
