using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;

namespace XCOMModHelper
{
    public class XCOMInstallManager
    {
        #region Properties

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public string XCOMInstallDirectory { get; set; }

        #endregion Properties

        public XCOMInstallManager()
        {            
        }

        public void Initialize()
        {
            if (string.IsNullOrEmpty(XCOMInstallDirectory))
            {
                XCOMInstallDirectory = GetXCOMInstallDirectory();
            }
            
            ValidateXCOMInstallDirectory();
            Log.Info("Using XCOM Installation Directory of [ {0} ]", XCOMInstallDirectory);
        }

        public XCOMInstallInfo GetInstallInfo(XCOMProductType productType)
        {
            Log.Debug("Getting XCOM Install Info for [ {0} ]", productType);
            XCOMInstallInfo info;
            switch (productType)
            {
                case XCOMProductType.XCOMEnemyUnknown:
                    info = GetEnemyUnknownInstallInfo();
                    break;
                case XCOMProductType.XCOMEnemyWithin:
                    info = GetEnemyWithinInstallInfo();
                    break;
                default:
                    throw new NotSupportedException(string.Format("Product Type [ {0} ] is not supported", productType));
            }

            Log.Debug("Install Info:\n{0}", info);

            return info;
        }

        #region Internal Methods

        private XCOMInstallInfo GetEnemyUnknownInstallInfo()
        {
            var gameInstallDirectory = XCOMInstallDirectory;
            var installInfo = new XCOMInstallInfo(XCOMProductType.XCOMEnemyUnknown)
            {
                XCOMGameInstallDirectory = gameInstallDirectory,
                XCOMBinaryDirectory = Path.Combine(gameInstallDirectory, Common.DefaultXCOMBinaryDirectory),
                XCOMExecutablePath =
                    Path.Combine(gameInstallDirectory, Common.DefaultXCOMBinaryDirectory, Common.DefaultXCOMEUExecutableName)
            };

            return installInfo;
        }

        private XCOMInstallInfo GetEnemyWithinInstallInfo()
        {
            var gameInstallDirectory = Path.Combine(XCOMInstallDirectory, Common.DefaultXCOMEWInstallDirectoryName);
            if (!Directory.Exists(gameInstallDirectory))
            {
                throw new InvalidOperationException(string.Format("The Enemy Within Directory [ {0} ] is not Valid", gameInstallDirectory));    
            }

            var installInfo = new XCOMInstallInfo(XCOMProductType.XCOMEnemyUnknown)
            {
                XCOMGameInstallDirectory = gameInstallDirectory,
                XCOMBinaryDirectory = Path.Combine(gameInstallDirectory, Common.DefaultXCOMBinaryDirectory),
                XCOMExecutablePath =
                    Path.Combine(gameInstallDirectory, Common.DefaultXCOMBinaryDirectory, Common.DefaultXCOMEWExecutableName)
            };
            return installInfo;
        }

        private void ValidateXCOMInstallDirectory()
        {
            // Check to make sure the path is actually valid and not garbage
            try
            {
                // get the full path
                var installDir = Path.GetFullPath(XCOMInstallDirectory);
                XCOMInstallDirectory = installDir;
            }
            catch (Exception err)
            {
                Log.Debug(err);
                throw new InvalidOperationException(string.Format("XCOM Install Directory [ {0} ] is not a valid path", XCOMInstallDirectory));
            }

            // Check to see if the directory exists
            Log.Debug("Found XCOM Full Install Path of [ {0} ]", XCOMInstallDirectory);
            if (!Directory.Exists(XCOMInstallDirectory))
            {
                throw new InvalidOperationException(string.Format("XCOM Install Directory [ {0} ] does not exist", XCOMInstallDirectory));
            }

        }

        private string GetXCOMInstallDirectory()
        {
            Log.Info("Attempting to Locate XCOM Install Directory");

            var installDirectory = Path.Combine(SteamUtilities.GetSteamRootDirectory(), Common.XCOMSteamAppsPath);
            Log.Debug("Checking for Default Install Location [ {0} ]", installDirectory);
            if (!Directory.Exists(installDirectory))
            {
                Log.Debug("XCOM Not Installed to Default Location, checking Config.VDF");
                installDirectory = GetXCOMInstallDirectoryFromVDFConfig();
            }

            return installDirectory;
        }

        private string GetXCOMInstallDirectoryFromVDFConfig()
        {
            var configPath = SteamUtilities.GetSteamConfigVDFPath();
            Log.Debug("Attempting to determine XCOM Install Directory from Config VDF [ {0} ]", configPath);

            var lines = File.ReadAllLines(configPath).Where(s => s.Contains(Common.XCOMInstallFolderName)).ToList();
            Log.Debug("Found Entries in VDF:\n{0}", string.Join("\n>", lines));

            if (lines.Count != 1)
            {
                throw new InvalidOperationException("Cannot Determine XCOM Install Directory from Config.VDF");
            }

            // format should be similar to: 
            // 						"installdir"		"X:\\Steam\\steamapps\\common\\XCom-Enemy-Unknown"

            var regex = new Regex(@"[\""]installdir[\""]\s+[\""](?<installdir>.+)[\""]", RegexOptions.IgnoreCase);
            var match = regex.Match(lines.First());

            var installDirectory = match.Groups["installdir"].Value;
            Log.Debug("Regex Matched Install Directory of [ {0} ]", installDirectory);

            return installDirectory;
        }

        #endregion Internal Methods
    }
}