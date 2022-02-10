using Algorand.V2.Algod.Model;
using Algorand.V2.Indexer.Model;
using System.Linq;

namespace Algorand.Common {

	/// <summary>
	/// <see cref="Transaction"/> factory methods.
	/// </summary>
	public static class TxnFactory {

		/// <summary>
		/// Create an application call transaction.
		/// </summary>
		/// <param name="from">Sender address</param>
		/// <param name="application">Application ID</param>
		/// <param name="txParams">Network parameters</param>
		/// <param name="onCompletion">On complete action</param>
		/// <param name="fee">Transaction fee</param>
		/// <param name="foreignApps">Foreign application IDs</param>
		/// <param name="foreignAssets">Foreign asset IDs</param>
		/// <param name="applicationArgs">Application arguments</param>
		/// <param name="note">Transaction note</param>
		/// <returns></returns>
		public static Transaction AppCall(
			Address from,
			ulong application,
			TransactionParametersResponse txParams,
			OnCompletion onCompletion = OnCompletion.Noop,
			ulong? fee = null,
			ulong[] foreignApps = null,
			ulong[] foreignAssets = null,
			byte[][] applicationArgs = null,
			byte[] note = null) {

			var result = Algorand.Utils.GetApplicationCallTransaction(
				from, application, txParams);

			if (fee.HasValue) {
				result.fee = fee.Value;
			}

			if (foreignApps != null) {
				result.foreignApps = foreignApps.ToList();
			}

			if (foreignAssets != null) {
				result.foreignAssets = foreignAssets.ToList();
			}

			if (applicationArgs != null) {
				result.applicationArgs = applicationArgs.ToList();
			}

			if (note != null) {
				result.note = note;
			}

			return null;
		}

		/// <summary>
		/// Create an application opt-in transaction.
		/// </summary>
		/// <param name="from">Sender address</param>
		/// <param name="application">Application ID</param>
		/// <param name="txParams">Network parameters</param>
		/// <returns>Application opt-in transaction</returns>
		public static Transaction AppOptIn(
			Address from,
			ulong application,
			TransactionParametersResponse txParams) {

			return Algorand.Utils.GetApplicationOptinTransaction(from, application, txParams);
		}

		/// <summary>
		/// Create a payment transaction
		/// </summary>
		/// <param name="from">Sender address</param>
		/// <param name="to">Recipient address</param>
		/// <param name="amount">Amount of asset</param>
		/// <param name="txParams">Network parameters</param>
		/// <param name="fee">Transaction fee</param>
		/// <param name="note">Transaction note</param>
		/// <returns>Payment transaction</returns>
		public static Transaction Pay(
			Address from,
			Address to,
			ulong amount,
			TransactionParametersResponse txParams,
			ulong? fee = null,
			byte[] note = null) {

			return Pay(from, to, amount, null, txParams, fee, note);
		}

		/// <summary>
		/// Create a payment or asset transfer transaction.
		/// </summary>
		/// <param name="from">Sender address</param>
		/// <param name="to">Recipient address</param>
		/// <param name="asset">Asset ID (0 or null for $ALGO)</param>
		/// <param name="amount">Amount of asset</param>
		/// <param name="txParams">Network parameters</param>
		/// <param name="fee">Transaction fee</param>
		/// <param name="note">Transaction note</param>
		/// <returns>Payment or asset transfer transaction</returns>
		public static Transaction Pay(
			Address from,
			Address to,
			ulong amount,
			ulong? asset,
			TransactionParametersResponse txParams,
			ulong? fee = null,
			byte[] note = null) {

			Transaction result = null;

			if (!asset.HasValue || asset == 0) {
				result = Algorand.Utils.GetPaymentTransaction(
					from, to, amount, null, txParams);
			} else {
				result = Algorand.Utils.GetTransferAssetTransaction(
					from, to, asset, amount, txParams);
			}

			if (fee.HasValue) {
				result.fee = fee.Value;
			}

			if (note != null) {
				result.note = note;
			}

			return result;
		}

	}

}
