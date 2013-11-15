using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using NLog;
using XCOMModHelper.Patcher;
using XCOMModHelper.Util;

namespace XCOMModHelper
{
    public class XCOMModHelper
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public bool IsVerbose { get; set; }

        public bool IsPatchTest { get; set; }

        public string XCOMRootDirectory { get; set; }

        public string ConfigurationFile { get; set; }

        public PatcherConfiguration PatcherConfig { get; private set; }

        public XCOMModHelper()
        {
            IsPatchTest = false;
        }

        public bool Execute()
        {
            Log.Debug("Initializing");
            
            GetXCOMRootDirectory();

            PatcherConfig = PatcherConfiguration.LoadConfiguration(ConfigurationFile);
            ExpandPatchTargetPaths();
            
            ApplyPatches();
            
            //BackupExecutable();
            //ApplyPatches();
            //Patcher.SaveChanges();
            return true;
        }

        #region Internal Methods

        private void GetXCOMRootDirectory()
        {
            string fullPath;
            if (string.IsNullOrEmpty(XCOMRootDirectory))
            {
                XCOMRootDirectory = XCOMUtilities.GetXCOMRootDirectory();
            }            
            try
            {
                fullPath = Path.GetFullPath(XCOMRootDirectory);
            }
            catch (Exception)
            {
                throw new InvalidOperationException(string.Format("The provided XCOM Root Path is not valid [ {0} ]", XCOMRootDirectory));                
            }

            XCOMRootDirectory = fullPath;

            if (!Directory.Exists(XCOMRootDirectory))
            {
                throw new DirectoryNotFoundException(string.Format("The Specified XCOM Root Directory does not exist, [ {0} ]", XCOMRootDirectory));
            }
            Log.Info("Found XCOM Root Directory of [ {0} ]", XCOMRootDirectory);
        }

        #region Configuration Path Expansion

        private void ExpandPatchTargetPaths()
        {
            Log.Debug("Expanding Target Paths");
            if (string.IsNullOrEmpty(PatcherConfig.BackupDirectory))
            {
                throw new InvalidOperationException("A valid backup directory must be provided");
            }

            var backupDirectory = Path.Combine(XCOMRootDirectory, PatcherConfig.BackupDirectory);

            Log.Info("Backup Target Directory is [ {0} ]", backupDirectory);
            if (!Directory.Exists(backupDirectory))
            {
                Log.Info("Backup Directory does not exist, creating");
                Directory.CreateDirectory(backupDirectory);
            }
            PatcherConfig.BackupDirectory = backupDirectory;

            if (string.IsNullOrEmpty(PatcherConfig.DecompressedUPKOutputDirectory))
            {
                throw new InvalidOperationException("A valid UPK output directory must be provided");
            }

            var decompressedUPKOutputDirectory = Path.GetFullPath(Path.Combine(XCOMRootDirectory, PatcherConfig.DecompressedUPKOutputDirectory));
            Log.Info("Decompressed UPK Folder [ {0} ]", decompressedUPKOutputDirectory);
            if (!Directory.Exists(decompressedUPKOutputDirectory))
            {
                Log.Info("Decompressed UPK Output Directory does not exist, creating");
                Directory.CreateDirectory(backupDirectory);
            }
            PatcherConfig.DecompressedUPKOutputDirectory = decompressedUPKOutputDirectory;
            

            // Adjust each target path of the patches
            foreach (var patch in PatcherConfig.Targets)
            {
                var targetPath = ExpandTargetPath(patch.TargetPath);
                patch.TargetPath = targetPath;
            }
            
        }

        private string ExpandTargetPath(string targetPath, bool validatePath=true)
        {
            var result = Path.Combine(XCOMRootDirectory, targetPath);
            Log.Debug("Expanded Target from [ {0} => {1} ]", targetPath, result);

            if (validatePath)
            {
                if (!File.Exists(result))
                {
                    throw new FileNotFoundException(string.Format("Cannot find the specified Target Path [ {0} ]", result));
                }
            }
            return result;
        }

        #endregion Configuration Path Expansion

        #region Patching

        private void ApplyPatches()
        {
            Log.Info("Patching [{0}] Targets", PatcherConfig.Targets.Count);
            foreach (var target in PatcherConfig.Targets)
            {
                PatchTarget(target);
            }

            //foreach (var patchEntry in PatcherConfig.Patches)
            //{
            //    Log.Info("Applying Patch:\n{0}", patchInfo);
            //}

            //Log.Info("Applying Patches #{0}", PatcherConfig.Patches.Count);
            //foreach (var patchEntry in PatcherConfig.Patches)
            //{
            //    Log.Info("Applying Patch:\n{0}", patchInfo);

            //    // Create patcher for specific file type
            //    IFilePatcher patcher = !patchInfo.IsUPKFile
            //        ? new FilePatcher(patchInfo.TargetPath)
            //        : new UPKPatcher(patchInfo.TargetPath);
            //    patcher.LoadFile();

            //    var findBytes = patchInfo.FindValue.GetByteArrayFromHexString();
            //    var replaceBytes = patchInfo.ReplaceValue.GetByteArrayFromHexString();

            //    var patchState = patcher.ApplyPatch(findBytes, replaceBytes);
                
            //    Log.Debug("Patch State ==> {0}", patchState);
            //    HandlePatchState(patchState, patchInfo);

            //    // We don't want to do anything since it has already been applied
            //    if (patchState == PatchState.AlreadyPatched)
            //    {
            //        Log.Info("Skipping Patch since it was Already Applied [ {0} ]", patchInfo);
            //        continue;
            //    }
                
            //    BackupFiles(patcher.FilesToBackup);
            //    Log.Info("Saving Patch Changes to [ {0} ]", patchInfo.TargetPath);
            //    if (!IsPatchTest)
            //    {
            //        patcher.WriteChanges(); 
            //    }
            //    Log.Info("Successfully applied patch [ {0} ]", patchInfo.Description);
            //}
        }

        private void PatchTarget(PatchTarget target)
        {
            Log.Info("Patching Target [ {0} ]", target.TargetPath);
            var isPatchedSuccessfully = false;
            
            // Create patcher for specific file type
            IFilePatcher patcher = !target.IsUPKFile
                ? new FilePatcher(target.TargetPath)
                : new UPKPatcher(target.TargetPath, PatcherConfig.DecompressedUPKOutputDirectory);
            
            patcher.LoadFile();
            
            foreach (var patch in target.PatchEntries)
            {
                Log.Info("Applying Patch [ {0} ]", patch.Description);

                var findBytes = patch.FindValue.GetByteArrayFromHexString();
                var replaceBytes = patch.ReplaceValue.GetByteArrayFromHexString();

                var patchState = patcher.ApplyPatch(findBytes, replaceBytes);
                Log.Debug("Patch State ==> {0}", patchState);
                HandlePatchState(patchState, patch);

                // We don't want to do anything since it has already been applied
                if (patchState == PatchState.AlreadyPatched)
                {
                    Log.Info("Skipping Patch since it was Already Applied [ {0} ]", patch.Description);
                    continue;
                }
                isPatchedSuccessfully = true;
            }

            if (isPatchedSuccessfully)
            {
                BackupFiles(patcher.FilesToBackup);
                Log.Info("Saving Changes to [ {0} ]", target.TargetPath);
                if (!IsPatchTest)
                {
                    patcher.WriteChanges();
                }
            }
            
        }

        private void BackupFiles(IEnumerable<string> filesToBackup)
        {
            foreach (var targetFilePath in filesToBackup)
            {
                var fileName = Path.GetFileName(targetFilePath);
                if (fileName == null)
                {
                    throw new InvalidOperationException(string.Format("Cannot get Filename from [ {0} ] for backup", targetFilePath));
                }
                var backupFilePath = Path.Combine(PatcherConfig.BackupDirectory, fileName);
                File.Copy(targetFilePath, backupFilePath, true);
                
                Log.Info("File [ {0} ] Backed up to [ {0} ]", targetFilePath, backupFilePath);                
            }
        }

        private static void HandlePatchState(PatchState state, PatchEntry patch)
        {
            switch (state)
            {
                case PatchState.Success:
                    Log.Info("Patch Successfully Applied [ {0} ]", patch.Description);
                    break;
                case PatchState.AlreadyPatched:
                    Log.Info("Patch was Already Applied [ {0} ]", patch.Description);
                    break;
                case PatchState.NotUnique:
                    throw new InvalidOperationException(string.Format("Patch was not unique, multiple matches were found, [ {0} ]", patch.Description));
                case PatchState.SequenceNotFound:
                    throw new InvalidOperationException(string.Format("Could not find the specified hex value for patch [ {0} ]", patch.Description));
            }
        }

        //private void ApplyPatches()
        //{
        //    Log.Info("Applying Patches to [ {0} ]", InstallInfo.XCOMExecutablePath);

        //    // Apply all of the patches to the executable
        //    foreach (var patch in ConfigManager.Patches)
        //    {
        //        var find = patch.Item1;
        //        var replace = patch.Item2;
        //        var findBytes = find.GetByteArrayFromHexString();
        //        var replaceBytes = replace.GetByteArrayFromHexString();

        //        Log.Debug("Patching\nFind> {0}\nReplace>{1}", find, replace);

        //        //TestConversion(find, replace);
        //        //Log.Debug("HF> {0}", Utilities.PadHexString(find));
        //        //Log.Debug("HR> {0}", Utilities.PadHexString(replace));
        //        //Log.Debug("BF> {0}", string.Join(" ", findBytes.Select(b => string.Format("{0,3}", b.ToString(CultureInfo.InvariantCulture)))));
        //        //Log.Debug("BR> {0}", string.Join(" ", replaceBytes.Select(b => string.Format("{0,3}", b.ToString(CultureInfo.InvariantCulture)))));

        //        Patcher.ApplyPatch(findBytes, replaceBytes);                
        //    }

        //    Log.Info("Patching of [ {0} ] was successful", InstallInfo.XCOMExecutablePath);
        //}

        #endregion Patching

        #endregion Internal Methods



    }
}