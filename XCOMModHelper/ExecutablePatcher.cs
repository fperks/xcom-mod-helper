using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XCOMModHelper
{
    public class ExecutablePatcher
    {
        private byte[] _executableData;

        public byte[] ExecutableData
        {
            get
            {
                if (_executableData == null)
                {
                    // Load from the file directly
                    _executableData = File.ReadAllBytes(ExecutablePath);
                }
                return _executableData;
            }
        }

        public string ExecutablePath { get; set; }
        
        public ExecutablePatcher(string executablePath)
        {
            ExecutablePath = executablePath;
        }

        /// <summary>
        /// Save the changes to the output file
        /// </summary>
        public void SaveChanges()
        {
            File.WriteAllBytes(ExecutablePath, ExecutableData);
        }
        
        public void ApplyPatch(byte[] pattern, byte[] patchData)
        {
            var indexes = ExecutableData.FindSequenceIndexes(pattern).ToList();
            if (!indexes.Any())
            {
                throw new InvalidOperationException(string.Format("The Sequence [ {0} ] was not found", pattern.GetHexStringFromByteArray()));
            }

            if (indexes.Count > 1)
            {
                throw new InvalidOperationException(string.Format("The Sequence [ {0} ] is not unique, # of Matches Found {1}", pattern.GetHexStringFromByteArray(), indexes.Count));
            }

            var index = indexes.First();
            Console.WriteLine("Patching at Index [ {0} ]", index);
            Array.Copy(patchData, 0, ExecutableData, index, patchData.Length);
        }
    }
}
