using Algorand.V2.Algod;
using Algorand.V2.Algod.Model;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Algorand.Common {

	public static class DefaultApiExtensions {

        /// <summary>
        /// Submit a <see cref="TransactionGroup"/>
        /// </summary>
        /// <param name="client">Algod REST API Client</param>
        /// <param name="txns">Transaction group</param>
        /// <param name="wait">Whether or not to wait for the transactions to be confirmed</param>
        /// <returns>The post transaction response</returns>
        public static async Task<PostTransactionsResponse> SubmitTransactionGroupAsync(
             this IDefaultApi client, TransactionGroup txns, bool wait = true) {

			if (client is null) {
				throw new ArgumentNullException(nameof(client));
			}

			if (txns is null) {
				throw new ArgumentNullException(nameof(txns));
			}

            var bytes = txns.EncodeToMsgPack();

            PostTransactionsResponse response = null;

            using (var payload = new MemoryStream(bytes.ToArray())) {
                response = await client.TransactionsAsync(payload);
            }

            if (wait) {
                await WaitForTransactionToComplete(client, response.TxId);
            }

            return response;
		}

        /// <summary>
        /// Wait for the transaction to be confirmed
        /// </summary>
        /// <param name="client">Algod Rest API client</param>
        /// <param name="txId">Transaction ID</param>
        /// <param name="timeout">Round timeout</param>
        /// <returns>The pending transaction response</returns>
        public static async Task<PendingTransactionResponse> WaitForTransactionToComplete(
            this IDefaultApi client, string txId, ulong timeout = 3) {

            // Source: https://github.com/RileyGe/dotnet-algorand-sdk/blob/345ac4a540abb3f8094b2f97be81bcef3ff6f385/dotnet-algorand-sdk/Util/Utils.cs#L47

            if (client is null) {
				throw new ArgumentNullException(nameof(client));
			}

			if (string.IsNullOrWhiteSpace(txId)) {
				throw new ArgumentException($"'{nameof(txId)}' cannot be null or whitespace.", nameof(txId));
			}

			var response = await client.StatusAsync();
            var start = response.LastRound + 1;
            var current = start;

            while (current < (start + timeout)) {
                var pendingInfo = await client.PendingGetAsync(txId, null);

                if (pendingInfo != null) {
                    if (pendingInfo.ConfirmedRound != null && pendingInfo.ConfirmedRound > 0) {
                        // Got the completed Transaction
                        return pendingInfo;
                    }
                    if (pendingInfo.PoolError != null && pendingInfo.PoolError.Length > 0) {
                        // If there was a pool error, then the transaction has been rejected!
                        throw new Exception("The transaction has been rejected with a pool error: " + pendingInfo.PoolError);
                    }
                }

                await client.WaitForBlockAfterAsync(current);
                current++;
            }

            throw new Exception("Transaction not confirmed after " + timeout + " rounds!");
        }

    }

}
