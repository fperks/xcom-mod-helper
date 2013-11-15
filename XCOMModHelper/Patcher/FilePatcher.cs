using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using XCOMModHelper.Util;

namespace XCOMModHelper.Patcher
{
    public class FilePatcher : IFilePatcher
    {
        #region Properties

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public string FileTargetPath { get; private set; }

        public List<string> FilesToBackup { get; protected set; }

        public byte[] FileData { get; protected set; }

        

        #endregion Properties

        #region Methods

        public FilePatcher(string filepath)
        {
            FileTargetPath = filepath;
            FilesToBackup = new List<string>();
        }

        public virtual void LoadFile()
        {
            Log.Debug("Loading File Data [ {0} ]", FileTargetPath);
            FileData = File.ReadAllBytes(FileTargetPath);
            Log.Debug("File [ {0} ] Loaded Into Patcher Successfully", FileTargetPath);
            FilesToBackup.Add(FileTargetPath);
        }

        public PatchState ApplyPatch(byte[] pattern, byte[] patchData)
        {
            var indexes = GetIndexesForPattern(pattern);
            if (!indexes.Any())
            {
                // check to see if it already exists
                var patchIndexes = GetIndexesForPattern(patchData);
                if (!patchIndexes.Any())
                {
                    return PatchState.SequenceNotFound;
                }
                else
                {
                    return PatchState.AlreadyPatched;
                }

                //throw new InvalidOperationException(string.Format("The Sequence [ {0} ] was not found", pattern.GetHexStringFromByteArray()));
            }

            if (indexes.Count > 1)
            {
                return PatchState.NotUnique;
                //throw new InvalidOperationException(string.Format("The Sequence [ {0} ] is not unique, # of Matches Found {1}", pattern.GetHexStringFromByteArray(), indexes.Count));
            }

            var index = indexes.First();
            Log.Info("Applying Patch at Index [ {0} ]", index);
            Array.Copy(patchData, 0, FileData, index, patchData.Length);
            return PatchState.Success;
        }

        public virtual void WriteChanges()
        {
            Log.Debug("Writing Changes to Patch Target [ {0} ]", FileTargetPath);
            File.WriteAllBytes(FileTargetPath, FileData);
        }

        #endregion Methods

        #region Internal Methods

        protected List<int> GetIndexesForPattern(byte[] pattern)
        {
            return FileData.FindSequenceIndexes(pattern).ToList();
        }

        #endregion Internal Methods
    }
}