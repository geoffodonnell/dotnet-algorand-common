using Algorand.Algod.Model;
using Algorand.Algod.Model.Transactions;
using System.Linq;

namespace Algorand.Common {

	/// <summary>
	/// <see cref="Transaction"/> factory methods.
	/// </summary>
	public static class TxnFactory {

		private const ulong MinFee = 1000;

		/// <summary>
		/// Create an application call transaction.
		/// </summary>
		/// <param name="from">Sender address</param>
		/// <param name="application">Application ID (0 for creation)</param>
		/// <param name="txParams">Network parameters</param>
		/// <param name="onCompletion">On complete action</param>
		/// <param name="fee">Transaction fee</param>
		/// <param name="foreignApps">Foreign application IDs</param>
		/// <param name="foreignAssets">Foreign asset IDs</param>
		/// <param name="accounts">Accounts</param>
		/// <param name="applicationArgs">Application arguments</param>
		/// <param name="note">Transaction note</param>
		/// <param name="approvalProgram">Approval program</param>
		/// <param name="clearStateProgram">Clear program</param>
		/// <param name="globalStateSchema">Global State Schema</param>
		/// <param name="localStateSchema">Local State Schema</param>
		/// <param name="rekeyTo">Rekey to address</param>
		/// <returns>Application call transaction</returns>
		public static Transaction AppCall(
			Address from,
			ulong application,
			TransactionParametersResponse txParams,
			OnCompletion onCompletion = OnCompletion.Noop,
			ulong? fee = null,
			ulong[] foreignApps = null,
			ulong[] foreignAssets = null,
			Address[] accounts = null,
			byte[][] applicationArgs = null,
			byte[] note = null,
			TEALProgram approvalProgram = null,
			TEALProgram clearStateProgram = null,
			StateSchema globalStateSchema = null,
			StateSchema localStateSchema = null,
			Address rekeyTo = null) {

			ApplicationCallTransaction result = null;

			switch (onCompletion) {
				case OnCompletion.Noop:
					if (application == 0) {
						result = new ApplicationCreateTransaction() {
							ApprovalProgram = approvalProgram,
							ClearStateProgram = clearStateProgram,
							GlobalStateSchema = globalStateSchema,
							LocalStateSchema = localStateSchema
						};
					} else {
						result = new ApplicationNoopTransaction() {
							ApplicationId = application
						};
					}					
					break;
				case OnCompletion.Optin:
					result = new ApplicationOptInTransaction() {
						ApplicationId = application
					};
					break;
				case OnCompletion.Closeout:
					result = new ApplicationCloseOutTransaction() {
						ApplicationId = application
					};
					break;
				case OnCompletion.Clear:
					result = new ApplicationClearStateTransaction() {
						ApplicationId = application
					};
					break;
				case OnCompletion.Update:
					result = new ApplicationUpdateTransaction() {
						ApplicationId = application,
						ApprovalProgram = approvalProgram,
						ClearStateProgram = clearStateProgram,
						GlobalStateSchema = globalStateSchema,
						LocalStateSchema = localStateSchema
					}; 
					break;
				case OnCompletion.Delete:
					result = new ApplicationDeleteTransaction() {
						ApplicationId = application
					};
					break;
			}

			result.Sender = from;
			result.Fee = txParams.Fee >= MinFee ? txParams.Fee : MinFee;
			result.GenesisID = txParams.GenesisId;
			result.GenesisHash = new Digest(txParams.GenesisHash);
			result.FirstValid = txParams.LastRound;
			result.LastValid = txParams.LastRound + 1000;

			if (fee.HasValue) {
				result.Fee = fee.Value;
			}

			if (foreignApps != null) {
				result.ForeignApps = foreignApps.ToList();
			}

			if (foreignAssets != null) {
				result.ForeignAssets = foreignAssets.ToList();
			}

			if (accounts != null) {
				result.Accounts = accounts.ToList();
			}

			if (applicationArgs != null) {
				result.ApplicationArgs = applicationArgs.ToList();
			}

			if (note != null) {
				result.Note = note;
			}

			if (rekeyTo != null) {
				result.RekeyTo = rekeyTo;
			}

			return result;
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

			return new ApplicationOptInTransaction() {
				Sender = from,
				ApplicationId = application,
				Fee = txParams.Fee >= MinFee ? txParams.Fee : MinFee,
				GenesisID = txParams.GenesisId,
				GenesisHash = new Digest(txParams.GenesisHash),
				FirstValid = txParams.LastRound,
				LastValid = txParams.LastRound + 1000
			};
		}

		/// <summary>
		/// Create an asset create transaction
		/// </summary>
		/// <param name="assetParams">Asset parameters</param>
		/// <param name="txParams">Network parameters</param>
		/// <returns>Asset create transaction</returns>
		public static Transaction AssetCreate(
			AssetParams assetParams,
			TransactionParametersResponse txParams) {

			return new AssetCreateTransaction {
				Sender = assetParams.Creator,
				AssetParams = assetParams,
				Fee = txParams.Fee >= MinFee ? txParams.Fee : MinFee,
				GenesisID = txParams.GenesisId,
				GenesisHash = new Digest(txParams.GenesisHash),
				FirstValid = txParams.LastRound,
				LastValid = txParams.LastRound + 1000
			};
		}

		/// <summary>
		/// Create an asset opt-in transaction
		/// </summary>
		/// <param name="from">Sender address</param>
		/// <param name="asset">Asset ID</param>
		/// <param name="txParams">Network parameters</param>
		/// <returns>Asset opt-in transaction</returns>
		public static Transaction AssetOptIn(
			Address from,
			ulong asset,
			TransactionParametersResponse txParams) {

			return new AssetAcceptTransaction() {
				XferAsset = asset,
				AssetReceiver = from,
				Sender = from,
				Fee = txParams.Fee >= MinFee ? txParams.Fee : MinFee,
				// Not populated in older SDK versions, leave it
				// unpopulated here to satisfy existing unit tests
				//GenesisID = txParams.GenesisId, 
				GenesisHash = new Digest(txParams.GenesisHash),
				FirstValid = txParams.LastRound,
				LastValid = txParams.LastRound + 1000
			};
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

			fee = fee ?? MinFee;

			Transaction result;

			if (!asset.HasValue || asset == 0) {
				result = PaymentTransaction.GetPaymentTransactionFromNetworkTransactionParameters(
					from, to, amount, null, txParams);
			} else {
				result = new AssetTransferTransaction() {
					XferAsset = asset,
					AssetAmount = amount,
					AssetReceiver = to,
					Sender = from,
					Fee = fee >= MinFee ? fee : MinFee,
					// Not populated in older SDK versions, leave it
					// unpopulated here to satisfy existing unit tests
					//GenesisID = txParams.GenesisId,
					GenesisHash = new Digest(txParams.GenesisHash),
					FirstValid = txParams.LastRound,
					LastValid = txParams.LastRound + 1000
				};
			}

			if (note != null) {
				result.Note = note;
			}

			return result;
		}

	}

}
