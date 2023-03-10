using Algorand.Algod.Model;
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

		private static Account Account01 = new Account(
			"autumn coach siege genius key " +
			"usual helmet wood stairs spatial " +
			"ridge holiday turn chief embody " +
			"exotic hotel arctic morning " +
			"boring beef such march absent update");

		private static Account Account02 = new Account(
			"join portion torch only rapid " +
			"beach pony moment rare illness " +
			"reunion hair chair album equip " +
			"such pencil merge turtle expect " +
			"suspect exact arrive about off");

		[TestMethod]
		public void TransactionGroup_Add_And_Sign() {

			var tx1 = TxnFactory.Pay(Account01.Address, (new Account()).Address, 5000000, TxParams);
			var tx2 = TxnFactory.Pay(Account01.Address, (new Account()).Address, 6000000, TxParams);

			var grp = new TransactionGroup(new[] { tx1 });

			grp.Add(tx2);
			grp.Sign(Account01);

			/* Verify length */
			Assert.AreEqual(grp.Transactions.Length, 2);
			Assert.AreEqual(grp.SignedTransactions.Length, 2);

			/* Verify the group is signed */
			Assert.AreEqual(grp.IsSigned, true);
		}

		[TestMethod]
		public void TransactionGroup_Add_And_Sign_RekeyTest() {

			var tx1 = TxnFactory.Pay(Account01.Address, (new Account()).Address, 5000000, TxParams);
			var tx2 = TxnFactory.Pay(Account01.Address, (new Account()).Address, 6000000, TxParams);
			var tx3 = TxnFactory.Pay(Account02.Address, (new Account()).Address, 7000000, TxParams);

			var grp = new TransactionGroup(new[] { tx1 });

			grp.Add(tx2);
			grp.Add(tx3);

			grp.Sign(Account01);

			/* Verify length */
			Assert.AreEqual(grp.Transactions.Length, 3);
			Assert.AreEqual(grp.SignedTransactions.Length, 3);

			/* Verify tx1 and tx2 are signed */
			Assert.IsNotNull(grp.SignedTransactions[0]);
			Assert.IsNotNull(grp.SignedTransactions[1]);
			Assert.IsNull(grp.SignedTransactions[2]);

			/* Verify the group is not signed yet */
			Assert.AreEqual(grp.IsSigned, false);

			grp.Sign(Account01, Account02.Address);

			/* Verify length */
			Assert.AreEqual(grp.Transactions.Length, 3);
			Assert.AreEqual(grp.SignedTransactions.Length, 3);

			/* Verify all txns are signed */
			Assert.IsNotNull(grp.SignedTransactions[0]);
			Assert.IsNotNull(grp.SignedTransactions[1]);
			Assert.IsNotNull(grp.SignedTransactions[2]);

			/* Verify the group is signed */
			Assert.AreEqual(grp.IsSigned, true);
		}

	}

}
