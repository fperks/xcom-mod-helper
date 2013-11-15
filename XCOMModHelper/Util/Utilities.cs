using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace XCOMModHelper.Util
{
    public static class Utilities
    {
        #region Hex Utilities


        public static void TestConversion(string find, string replace)
        {
            Console.WriteLine("HF> {0}", PadHexString(find));
            Console.WriteLine("HR> {0}", PadHexString(replace));

            var findBytes = find.GetByteArrayFromHexString();
            var replaceBytes = replace.GetByteArrayFromHexString();

            Console.WriteLine("BF> {0}", string.Join(" ", findBytes.Select(b => String.Format((string) "{0,3}", (object) b.ToString(CultureInfo.InvariantCulture)))));
            Console.WriteLine("BR> {0}", string.Join(" ", replaceBytes.Select(b => String.Format("{0,3}", b.ToString(CultureInfo.InvariantCulture)))));
        }

        public static string PadHexString(string source)
        {
            var tokens = source.Split(new[] { ' ' });

            // pad with leading zero
            return string.Join(" ", tokens.Select(s => string.Format("{0,3}", s)));
        }

        /// <summary>
        /// Converts a String representing hex into a byte array
        /// </summary>
        /// <param name="value">The Source String</param>
        /// <returns>A byte array representation</returns>
        public static byte[] GetByteArrayFromHexString(this String value)
        {
            value = value.Replace(" ", "");
            var count = value.Length;
            var result = new byte[count / 2];
            for (var i = 0; i < count; i += 2)
            {
                result[i / 2] = Convert.ToByte(value.Substring(i, 2), 16);
            }
            return result;

            //var parser = SoapHexBinary.Parse(value);
            //return parser.Value;
        }
        
        /// <summary>
        /// Converts a byte array into a string
        /// </summary>
        /// <param name="value">The source byte Array</param>
        /// <returns>Returns the string representation</returns>
        public static string GetHexStringFromByteArray(this byte[] value)
        {
            var result = BitConverter.ToString(value);
            return result.Replace("-", " ");
        }
        
        #endregion Hex Utilities

        /// <summary>
        /// Find all matching sequences within the given array
        /// </summary>
        /// <typeparam name="T">An object</typeparam>
        /// <param name="source">Source Array</param>
        /// <param name="sequence">The sequence we are searching for</param>
        /// <returns>Returns all the indexes of the sequence</returns>
        public static IEnumerable<int> FindSequenceIndexes<T>(this T[] source, T[] sequence)
        {
            for (var i = 0; i < source.Length; i++)
            {
                if (MatchesSequence(source, sequence, i))
                {
                    yield return i;
                }
            }
        }

        private static bool MatchesSequence<T>(T[] source, T[] sequence, int index)
        {
            if (sequence.Length > (source.Length - index))
            {
                return false;
            }

            for (var i = 0; i < sequence.Length; i++)
            {

                if (!source[index + i].Equals(sequence[i]))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
