using System.Collections.Generic;
using System.IO;
using NLog;

namespace XCOMModHelper.Patcher
{
    public class UPKPatcher : FilePatcher
    {
        #region Properties

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public bool HasUncompressedSizeFile { get; private set; }

        public string UPKDecompressDirectory { get; private set; }

        #endregion Properties

        public UPKPatcher(string filepath, string upkDecompressDirectory) : base(filepath)
        {
            UPKDecompressDirectory = upkDecompressDirectory;
        }

        public override void LoadFile()
        {
            //base.LoadFile();
            Log.Debug("Loading UPK File Data [ {0} ]", FileTargetPath);
            var uncompressedUpkFilePath = XCOMUtilities.DecompressUPKFile(UPKDecompressDirectory, FileTargetPath);

            // Check for uncompressed_size
            var uncompressedSizeFilePath = string.Format("{0}.uncompressed_size", FileTargetPath);
            Log.Debug("Checking for uncompressed_size [ {0} ]", uncompressedSizeFilePath);
            if (File.Exists(uncompressedSizeFilePath))
            {
                Log.Debug("uncompressed_size [ {0} ] file exists", uncompressedSizeFilePath);
                FilesToBackup.Add(uncompressedSizeFilePath);
                HasUncompressedSizeFile = true;
            }


            FileData = File.ReadAllBytes(uncompressedUpkFilePath);
            Log.Debug("File [ {0} ] Loaded Into Patcher Successfully", FileTargetPath);
            FilesToBackup.Add(FileTargetPath);
        }

        public override void WriteChanges()
        {
            foreach (var backupFilePath in FilesToBackup)
            {
                File.Delete(backupFilePath);
            }
            File.WriteAllBytes(FileTargetPath, FileData);
        }
    }
}