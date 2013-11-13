using System;
using System.Globalization;
using System.IO;
using System.Linq;
using NLog;

namespace XCOMModHelper
{
    public class XCOMModHelper
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public bool IsVerbose { get; set; }

        public XCOMProductType ProductType { get; set; }

        public string ConfigurationFile { get; set; }

        public XCOMInstallManager InstallManager { get; private set; }

        public ConfigurationManager ConfigManager { get; private set; }

        public ExecutablePatcher Patcher { get; private set; }

        public XCOMInstallInfo InstallInfo { get; private set; }

        public XCOMModHelper()
        {
            InstallManager = new XCOMInstallManager();
            ConfigManager = new ConfigurationManager();
        }

        public void Execute()
        {
            InstallManager.Initialize();
            InstallInfo = InstallManager.GetInstallInfo(ProductType);

            Log.Info("Loading Configuration File [ {0} ]", ConfigurationFile);
            ConfigManager.LoadConfiguration(ConfigurationFile);
            Patcher = new ExecutablePatcher(InstallInfo.XCOMExecutablePath);
            BackupExecutable();
            ApplyPatches();
            Patcher.SaveChanges();
        }

        private void BackupExecutable()
        {
            var backupFile = Path.Combine(InstallInfo.XCOMBinaryDirectory, 
                ProductType == XCOMProductType.XCOMEnemyUnknown ? Common.DefaultXCOMEUBackupExecutableName : Common.DefaultXCOMEWBackupExecutableName);
            Log.Info("Backing up XCOMGame to [ {0} ]", backupFile);
            File.Copy(InstallInfo.XCOMExecutablePath, backupFile, true);
        }

        private void ApplyPatches()
        {
            Log.Info("Applying Patches");

            // Apply all of the patches to the executable
            foreach (var patch in ConfigManager.Patches)
            {
                var find = patch.Item1;
                var replace = patch.Item2;
                var findBytes = find.GetByteArrayFromHexString();
                var replaceBytes = replace.GetByteArrayFromHexString();

                Log.Debug("Patching\nFind> {0}\nReplace>{1}", find, replace);

                //TestConversion(find, replace);
                //Log.Debug("HF> {0}", Utilities.PadHexString(find));
                //Log.Debug("HR> {0}", Utilities.PadHexString(replace));
                //Log.Debug("BF> {0}", string.Join(" ", findBytes.Select(b => string.Format("{0,3}", b.ToString(CultureInfo.InvariantCulture)))));
                //Log.Debug("BR> {0}", string.Join(" ", replaceBytes.Select(b => string.Format("{0,3}", b.ToString(CultureInfo.InvariantCulture)))));

                Patcher.ApplyPatch(findBytes, replaceBytes);                
            }

            Log.Info("Patches Applied Successfully");
        }

    }
}