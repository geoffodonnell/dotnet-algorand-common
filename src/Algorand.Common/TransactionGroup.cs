using Algorand.Algod.Model;
using Algorand.Algod.Model.Transactions;
using Algorand.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorand.Common {

	/// <summary>
	///  A batch operation to be submitted as a unit; all transactions in the batch either pass or fail.
	/// </summary>
	public class TransactionGroup {

		/// <summary>
		/// The transactions in the group
		/// </summary>
		public virtual Transaction[] Transactions { get; protected set; }

		/// <summary>
		/// The signed transactions in the group
		/// </summary>
		public virtual SignedTransaction[] SignedTransactions { get; protected set; }

		/// <summary>
		/// Whether or not all the transactions in the group have been signed
		/// </summary>
		public virtual bool IsSigned => SignedTransactions.All(s => s != null);

		/// <summary>
		/// Create a new <see cref="TransactionGroup"/> 
		/// </summary>
		/// <param name="transactions">The transactions in the group</param>
		public TransactionGroup(IEnumerable<Transaction> transactions) {

			Transactions = transactions.ToArray();
			SignedTransactions = new SignedTransaction[Transactions.Length];

			SetGroupIdOnAllTransactions();
		}

		/// <summary>
		/// Add a transaction to the transaction group
		/// </summary>
		/// <param name="tx">Transaction to add to the group</param>
		public virtual void Add(Transaction tx) {

			if (tx is null) {
				throw new ArgumentNullException(nameof(tx));
			}

			var txns = new List<Transaction>(Transactions) {
				tx
			};

			Transactions = txns.ToArray();
			SignedTransactions = new SignedTransaction[Transactions.Length];

			SetGroupIdOnAllTransactions();
		}

		/// <summary>
		/// Sign transactions using the provided <see cref="Account"/> instance.
		/// </summary>
		/// <param name="account">Account to perform signing</param>
		/// <param name="sender">Sender of transaction(s) to sign (if different than account)</param>
		public virtual void Sign(Account account, Address sender = null) {

			PerformSign(sender ?? account.Address, s => s.Sign(account));
		}

		/// <summary>
		/// Sign transactions using the provided <see cref="LogicsigSignature"/> instance.
		/// </summary>
		/// <param name="logicsig">LogicSig to perform signing</param>
		/// <param name="sender">Sender of transaction(s) to sign (if different than account)</param>
		public virtual void SignWithLogicSig(LogicsigSignature logicsig, Address sender = null) {

			PerformSign(sender ?? logicsig.Address, s => s.Sign(logicsig));
		}

		/// <summary>
		/// Sign transactions using the provided private key.
		/// </summary>
		/// <param name="privateKey"></param>
		/// <param name="sender">Sender of transaction(s) to sign (if different than account)</param>
		public virtual void SignWithPrivateKey(byte[] privateKey, Address sender = null) {

			var account = new Account(privateKey);

			PerformSign(sender ?? account.Address, s => s.Sign(account));
		}

		/// <summary>
		/// Encode the signed transactions into a single msg pack.
		/// </summary>
		/// <returns>Msg Pack</returns>
		public virtual byte[] EncodeToMsgPack() {

			if (!IsSigned) {
				throw new Exception(
					"Transaction group has not been signed.");
			}

			var bytes = new List<byte>();

			foreach (var tx in SignedTransactions) {
				bytes.AddRange(Encoder.EncodeToMsgPackOrdered(tx));
			}

			return bytes.ToArray();
		}

		/// <summary>
		/// Sets the Group Id on all transactions in the group
		/// </summary>
		protected virtual void SetGroupIdOnAllTransactions() {

			if (Transactions != null && Transactions.Length > 0) {
				TxGroup.AssignGroupID(Transactions);
			}
		}

		/// <summary>
		/// Perform the appropriate signing operation on transactions from the specified <paramref name="sender"/>.
		/// </summary>
		/// <param name="sender">Transaction sender</param>
		/// <param name="action">Signing action</param>
		protected virtual void PerformSign(
			Address sender, Func<Transaction, SignedTransaction> action) {

			if (Transactions == null || Transactions.Length == 0) {
				return;
			}

			for (var i = 0; i < Transactions.Length; i++) {
				if (Transactions[i].Sender.Equals(sender)) {
					var signed = action(Transactions[i]);
					SignedTransactions[i] = signed;
				}
			}
		}

	}

}
