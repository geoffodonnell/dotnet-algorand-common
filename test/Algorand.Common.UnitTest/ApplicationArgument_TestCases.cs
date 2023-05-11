using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Algorand.Common.UnitTest {

	[TestClass]
	public class ApplicationArgument_TestCases {

		[TestMethod]
		public void Number_EncodesValue_ReturnsCorrectByteArray() {

			ulong value1 = 0;
			byte[] expected1 = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
			ulong value2 = ulong.MaxValue;
			byte[] expected2 = new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
			ulong value3 = 123456789;
			byte[] expected3 = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x07, 0x5b, 0xcd, 0x15 };
			ulong value4 = 18446744073709551615;
			byte[] expected4 = new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };

			byte[] result1 = ApplicationArgument.Number(value1);
			byte[] result2 = ApplicationArgument.Number(value2);
			byte[] result3 = ApplicationArgument.Number(value3);
			byte[] result4 = ApplicationArgument.Number(value4);

			CollectionAssert.AreEqual(expected1, result1);
			CollectionAssert.AreEqual(expected2, result2);
			CollectionAssert.AreEqual(expected3, result3);
			CollectionAssert.AreEqual(expected4, result4);
		}

		[TestMethod]
		public void String_EncodesValue_ReturnsCorrectByteArray() {

			string value1 = "hello world";
			byte[] expected1 = new byte[] { 104, 101, 108, 108, 111, 32, 119, 111, 114, 108, 100 };

			string value2 = "";
			byte[] expected2 = new byte[0];

			string value3 = "12345";
			byte[] expected3 = new byte[] { 49, 50, 51, 52, 53 };

			string value4 = "special characters: ~`!@#$%^&*()_-+={[}]|\\:;\"'<,>.?/";
			byte[] expected4 = new byte[] { 115, 112, 101, 99, 105, 97, 108, 32, 99, 104, 97, 114, 97, 99, 116, 101, 114, 115, 58, 32, 126, 96, 33, 64, 35, 36, 37, 94, 38, 42, 40, 41, 95, 45, 43, 61, 123, 91, 125, 93, 124, 92, 58, 59, 34, 39, 60, 44, 62, 46, 63, 47 };

			byte[] result1 = ApplicationArgument.String(value1);
			byte[] result2 = ApplicationArgument.String(value2);
			byte[] result3 = ApplicationArgument.String(value3);
			byte[] result4 = ApplicationArgument.String(value4);

			CollectionAssert.AreEqual(expected1, result1);
			CollectionAssert.AreEqual(expected2, result2);
			CollectionAssert.AreEqual(expected3, result3);
			CollectionAssert.AreEqual(expected4, result4);
		}

	}

}