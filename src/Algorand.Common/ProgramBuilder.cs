using Algorand.Common.Asc;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorand.Common {

	public static class ProgramBuilder {

        /// <summary>
        /// Build program bytes from a logic definition and collection of variable values
        /// </summary>
        /// <param name="logic">Logic definition</param>
        /// <param name="variables">Collection of variables</param>
        /// <returns>Program bytes</returns>
        public static byte[] CreateProgram(
            ProgramLogic logic, Dictionary<string, object> variables) {

            var template = logic.ByteCode;
            var templateBytes = Base64.Decode(template).ToList();

            if (variables == null) {
                return templateBytes.ToArray();
            }

            var offset = 0;

            foreach (var variable in logic.Variables.OrderBy(s => s.Index)) {

                var name = variable.Name.Substring(5).ToLower();
                var value = variables[name];
                var start = variable.Index - offset;
                var end = start + variable.Length;
                var valueEncoded = EncodeValue(value, variable.Type);
                var valueEncodedLength = valueEncoded.Length;
                var diff = variable.Length - valueEncodedLength;
                offset += diff;

                templateBytes.RemoveRange(start, variable.Length);
                templateBytes.InsertRange(start, valueEncoded);
            }

            return templateBytes.ToArray();
        }

        /// <summary>
        /// Encode a value for use in a program
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="type">The value type</param>
        /// <returns>Byte representation of the value</returns>
        /// <remarks>
        /// At this time, only 'int' value types are supported
        /// </remarks>
        public static byte[] EncodeValue(object value, string type) {

            if (String.Equals(type, "int", StringComparison.OrdinalIgnoreCase)) {
                return EncodeNumber(value);
            }

            throw new ArgumentException($"Unsupported value type {type}. At this time, only 'int' is supported.");
        }

        /// <summary>
        /// Encode a number for use in a program
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>Byte representation of the value</returns>
        public static byte[] EncodeNumber(object value) {

            var result = new List<byte>();
            var number = Convert.ToUInt64(value);

            while (true) {
                var toWrite = number & 0x7F;

                number >>= 7;

                if (number > 0) {
                    result.Add((byte)(toWrite | 0x80));
                } else {
                    result.Add((byte)toWrite);
                    break;
                }
            }

            return result.ToArray();
        }

    }

}
