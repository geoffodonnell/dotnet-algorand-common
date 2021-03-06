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
		/// 
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
		/// <param name="tx"></param>
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
		/// <param name="account"></param>
		public virtual void Sign(Account account) {

			PerformSign(account.Address, s => account.SignTransaction(s));
		}

		/// <summary>
		/// Sign transactions using the provided <see cref="LogicsigSignature"/> instance.
		/// </summary>
		/// <param name="logicsig"></param>
		public virtual void SignWithLogicSig(LogicsigSignature logicsig) {

			PerformSign(logicsig.Address, s => SignLogicsigTransaction(logicsig, s));
		}

		/// <summary>
		/// Sign transactions using the provided private key.
		/// </summary>
		/// <param name="privateKey"></param>
		public virtual void SignWithPrivateKey(byte[] privateKey) {

			var account = Account.AccountFromPrivateKey(privateKey);

			PerformSign(account.Address, s => account.SignTransaction(s));
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
				bytes.AddRange(Encoder.EncodeToMsgPack(tx));
			}

			return bytes.ToArray();
		}

		/// <summary>
		/// Sets the Group Id on all transactions in the group
		/// </summary>
		protected virtual void SetGroupIdOnAllTransactions() {

			var gid = TxGroup.ComputeGroupID(Transactions);

			foreach (var tx in Transactions) {
				tx.AssignGroupID(gid);
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
				if (Transactions[i].sender.Equals(sender)) {
					var signed = action(Transactions[i]);
					SignedTransactions[i] = signed;
				}
			}
		}

		/// <summary>
		/// Sign the provided <see cref="Transaction"/> with the <see cref="LogicsigSignature"/>.
		/// </summary>
		/// <param name="logicsig">Logic Sig</param>
		/// <param name="tx">Transaction</param>
		/// <returns></returns>
		protected static SignedTransaction SignLogicsigTransaction(
			LogicsigSignature logicsig, Transaction tx) {

			try {
				return Account.SignLogicsigTransaction(logicsig, tx);
			} catch (Exception) {
				if (tx.sender.Equals(logicsig.Address)) {
					return new SignedTransaction(tx, logicsig, tx.TxID());
				}

				throw;
			}
		}

	}

}
