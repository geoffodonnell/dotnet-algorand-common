using Algorand.V2.Algod.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Algorand.Common.UnitTest {

	[TestClass]
	public class TransactionGroup_TestCases {

		public static TransactionParametersResponse TxParams = new TransactionParametersResponse {
			ConsensusVersion = "https://github.com/algorandfoundation/specs/tree/abc54f79f9ad679d2d22f0fb9909fb005c16f8a1",
			Fee = 0,
			GenesisHash = Base64.Decode(Strings.ToUtf8ByteArray("SGO1GKSzyE7IEPItTxCByw9x8FmnrCDexi9/cOUJOiI=")),
			GenesisId = "testnet-v1.0",
			LastRound = 10000,
			MinFee = 1000
		};

		private static Account Account = new Account(
			"autumn coach siege genius key " +
			"usual helmet wood stairs spatial " +
			"ridge holiday turn chief embody " +
			"exotic hotel arctic morning " +
			"boring beef such march absent update");

		[TestMethod]
		public void TransactionGroup_Add_And_Sign() {

			var tx1 = Utils.GetPaymentTransaction(Account.Address, (new Account()).Address, 5000000, "", TxParams);
			var tx2 = Utils.GetPaymentTransaction(Account.Address, (new Account()).Address, 6000000, "", TxParams);

			var grp = new TransactionGroup(new[] { tx1 });

			grp.Add(tx2);
			grp.Sign(Account);

			Assert.AreEqual(grp.Transactions.Length, 2);
			Assert.AreEqual(grp.SignedTransactions.Length, 2);
			Assert.AreEqual(grp.IsSigned, true);
		}

	}

}
