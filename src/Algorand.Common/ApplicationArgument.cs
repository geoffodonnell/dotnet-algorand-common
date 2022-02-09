using Org.BouncyCastle.Utilities;
using System.Buffers.Binary;

namespace Algorand.Common {

	/// <summary>
	/// Encode values to use in arguments of Application Call transactions.
	/// </summary>
	public static class ApplicationArgument {

		/// <summary>
		/// Encode a number
		/// </summary>
		/// <param name="value">Application argument</param>
		/// <returns>Byte array representing the application argument</returns>
		public static byte[] Number(ulong value) {

			var result = new byte[8];

			BinaryPrimitives.WriteUInt64BigEndian(result, value);

			return result;
		}

		/// <summary>
		/// Encode a string
		/// </summary>
		/// <param name="value">Application argument</param>
		/// <returns>Byte array representing the application argument</returns>
		public static byte[] String(string value) {

			return Strings.ToUtf8ByteArray(value);
		}

	}

}
