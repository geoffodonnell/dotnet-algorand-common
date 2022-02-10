using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorand.Common {

    /// <summary>
    /// Contains methods for working with application state (global and local).
    /// </summary>
	public static class ApplicationState {

        private static readonly StringComparison mCmp = StringComparison.InvariantCulture;

        /// <summary>
        /// Encode a number as a state key
        /// </summary>
        /// <param name="value">The state key</param>
        /// <returns>The encoded key string</returns>
        public static string EncodeKey(ulong value) {

            var paddingBytes = Encoding.UTF8.GetBytes("o");
            var valueBytes = new byte[8];

            BinaryPrimitives.WriteUInt64BigEndian(valueBytes, value);

            return Base64.ToBase64String(
                Join(paddingBytes, valueBytes));
        }

        /// <summary>
        /// Encode a string as a state key
        /// </summary>
        /// <param name="key">The state key</param>
        /// <returns>The encoded key string</returns>
        public static string EncodeKey(string key) {

            return Base64.ToBase64String(
                Strings.ToUtf8ByteArray(key));
        }

        /// <summary>
        /// Retrieve a local state value
        /// </summary>
        /// <param name="state">Local state collection</param>
        /// <param name="applicationId">Target application ID</param>
        /// <param name="key">State key</param>
        /// <returns>The state value, if the application ID and key are found, null otherwise</returns>
        public static string GetBytes(
            ICollection<Algorand.V2.Algod.Model.ApplicationLocalState> state, ulong applicationId, string key) {

            var applicationState = state.FirstOrDefault(s => s.Id == applicationId);

            if (applicationState == null) {
                return null;
            }

            return GetBytes(applicationState.KeyValue, key);
        }

        /// <summary>
        /// Retrieve a local state value
        /// </summary>
        /// <param name="state">Local state collection</param>
        /// <param name="applicationId">Target application ID</param>
        /// <param name="key">State key</param>
        /// <returns>The state value, if the application ID and key are found, null otherwise</returns>
        public static string GetBytes(
            ICollection<Algorand.V2.Indexer.Model.ApplicationLocalState> state, ulong applicationId, string key) {

            var applicationState = state.FirstOrDefault(s => s.Id == applicationId);

            if (applicationState == null) {
                return null;
            }

            return GetBytes(applicationState.KeyValue, key);
        }

        /// <summary>
        /// Retrieve a local state value
        /// </summary>
        /// <param name="state">Local state collection</param>
        /// <param name="applicationId">Target application ID</param>
        /// <param name="key">State key</param>
        /// <returns>The state value, if the application ID and key are found, null otherwise</returns>
        public static ulong? GetNumber(
            ICollection<Algorand.V2.Algod.Model.ApplicationLocalState> state, ulong applicationId, string key) {

            var applicationState = state.FirstOrDefault(s => s.Id == applicationId);

            if (applicationState == null) {
                return null;
            }

            return GetNumber(applicationState.KeyValue, key);
        }

        /// <summary>
        /// Retrieve a local state value
        /// </summary>
        /// <param name="state">Local state collection</param>
        /// <param name="applicationId">Target application ID</param>
        /// <param name="key">State key</param>
        /// <returns>The state value, if the application ID and key are found, null otherwise</returns>
        public static ulong? GetNumber(
            ICollection<Algorand.V2.Indexer.Model.ApplicationLocalState> state, ulong applicationId, string key) {

            var applicationState = state.FirstOrDefault(s => s.Id == applicationId);

            if (applicationState == null) {
                return null;
            }

            return GetNumber(applicationState.KeyValue, key);
        }

        /// <summary>
        /// Retrieve a state value
        /// </summary>
        /// <param name="state">State store</param>
        /// <param name="key">State key</param>
        /// <returns>The state value, if the key is found, null otherwise</returns>
        public static string GetBytes(
            Algorand.V2.Algod.Model.TealKeyValueStore state, string key) {

            var value = GetValue(state, key);

            if (value == null) {
                return null;
            }

            //TODO: Check type

            return value.Bytes;
        }

        /// <summary>
        /// Retrieve a state value
        /// </summary>
        /// <param name="state">State store</param>
        /// <param name="key">State key</param>
        /// <returns>The state value, if the key is found, null otherwise</returns>
        public static string GetBytes(
            Algorand.V2.Indexer.Model.TealKeyValueStore state, string key) {

            var value = GetValue(state, key);

            if (value == null) {
                return null;
            }

            //TODO: Check type

            return value.Bytes;
        }

        /// <summary>
        /// Retrieve a state value
        /// </summary>
        /// <param name="state">State store</param>
        /// <param name="key">State key</param>
        /// <returns>The state value, if the key is found, null otherwise</returns>
        public static ulong? GetNumber(
            Algorand.V2.Algod.Model.TealKeyValueStore state, string key) {

            var value = GetValue(state, key);

            if (value == null) {
                return null;
            }

            //TODO: Check type

            return value.Uint;
        }

        /// <summary>
        /// Retrieve a state value
        /// </summary>
        /// <param name="state">State store</param>
        /// <param name="key">State key</param>
        /// <returns>The state value, if the key is found, null otherwise</returns>
        public static ulong? GetNumber(
            Algorand.V2.Indexer.Model.TealKeyValueStore state, string key) {

            var value = GetValue(state, key);

            if (value == null) {
                return null;
            }

            //TODO: Check type

            return value.Uint;
        }

        /// <summary>
        /// Retrieve a state value
        /// </summary>
        /// <param name="state">State store as a dictionary</param>
        /// <param name="key">State key</param>
        /// <returns>The state value, if the key is found, null otherwise</returns>
        public static ulong? GetNumber(
            Dictionary<string, Algorand.V2.Algod.Model.TealValue> state, string key) {

            if (state.TryGetValue(key, out var value)) {
                return value.Uint;
            } else if (state.TryGetValue(EncodeKey(key), out value)) {
                return value.Uint;
            }

            return null;
        }

        /// <summary>
        /// Retrieve a state value
        /// </summary>
        /// <param name="state">State store as a dictionary</param>
        /// <param name="key">State key</param>
        /// <returns>The state value, if the key is found, null otherwise</returns>
        public static ulong? GetNumber(
            Dictionary<string, Algorand.V2.Indexer.Model.TealValue> state, string key) {

            if (state.TryGetValue(key, out var value)) {
                return value.Uint;
            } else if (state.TryGetValue(EncodeKey(key), out value)) {
                return value.Uint;
            }

            return null;
        }

        /// <summary>
        /// Retrieve a state value
        /// </summary>
        /// <param name="state">State store as a dictionary</param>
        /// <param name="key">State key</param>
        /// <returns>The state value, if the key is found, null otherwise</returns>
        public static string GetBytes(
            Dictionary<string, Algorand.V2.Algod.Model.TealValue> state, string key) {

            if (state.TryGetValue(key, out var value)) {
                return value.Bytes;
            } else if (state.TryGetValue(EncodeKey(key), out value)) {
                return value.Bytes;
            }

            return null;
        }

        /// <summary>
        /// Retrieve a state value
        /// </summary>
        /// <param name="state">State store as a dictionary</param>
        /// <param name="key">State key</param>
        /// <returns>The state value, if the key is found, null otherwise</returns>
        public static string GetBytes(
            Dictionary<string, Algorand.V2.Indexer.Model.TealValue> state, string key) {

            if (state.TryGetValue(key, out var value)) {
                return value.Bytes;
            } else if (state.TryGetValue(EncodeKey(key), out value)) {
                return value.Bytes;
            }

            return null;
        }

        private static Algorand.V2.Algod.Model.TealValue GetValue(
            Algorand.V2.Algod.Model.TealKeyValueStore state, string key) {

            var found = state.FirstOrDefault(s => String.Equals(s.Key, key, mCmp));

            if (found == null) {
                key = EncodeKey(key);
                found = state.FirstOrDefault(s => String.Equals(s.Key, key, mCmp));
            }

            return found?.Value;
        }

        private static Algorand.V2.Indexer.Model.TealValue GetValue(
            Algorand.V2.Indexer.Model.TealKeyValueStore state, string key) {

            var found = state.FirstOrDefault(s => String.Equals(s.Key, key, mCmp));

            if (found == null) {
                key = EncodeKey(key);
                found = state.FirstOrDefault(s => String.Equals(s.Key, key, mCmp));
            }

            return found?.Value;
        }

        private static T[] Join<T>(IEnumerable<T> a, IEnumerable<T> b) {

            var result = a.ToList();

            result.AddRange(b);

            return result.ToArray();
        }

    }

}
