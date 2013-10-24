using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace XCOMEUModHelper
{
    public static class Helpers
    {
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

        public static string GetXCOMRootDirectory()
        {
            var steamPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", string.Empty);
            if (string.IsNullOrEmpty(steamPath))
            {
                throw new InvalidOperationException("Cannot Retrieve The Path to Steam");
            }

            var result = Path.Combine(steamPath, @"SteamApps\common\XCom-Enemy-Unknown");
            if (!Directory.Exists(result))
            {
                throw new InvalidOperationException(string.Format("Cannot Find the XCOM Enemy Unknown Directory at [ {0} ]", result));
            }
            return result;
        }

        public static string GetXCOMExecutablePath(String xcomDirectory = null)
        {
            xcomDirectory = xcomDirectory ?? GetXCOMRootDirectory();
            var result = Path.Combine(xcomDirectory, @"Binaries\Win32\XComGame.exe");
            if (!File.Exists(result))
            {
                throw new InvalidOperationException(string.Format("Cannot Find the XCOM Enemy Unknown Executable Path at [ {0} ]", result));
            }
            return result;
        }


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