using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;
using XCOMModHelper.Util;

namespace XCOMModHelper
{
    public static class XCOMUtilities
    {
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static string GetXCOMRootDirectory()
        {
            Log.Info("Attempting to Locate XCOM Root Directory");
            var rootDirectory = Path.Combine(SteamUtilities.GetSteamRootDirectory(), Common.XCOMSteamAppsPath);
            Log.Debug("Checking for Default Install Location [ {0} ]", rootDirectory);
            if (!Directory.Exists(rootDirectory))
            {
                Log.Debug("XCOM Not Installed to Default Location, checking Config.VDF");
                rootDirectory = GetXCOMRootDirectoryFromVDFConfig();
            }

            // Get the absolute full path
            try
            {
                return Path.GetFullPath(rootDirectory);
            }
            catch (Exception err)
            {
                Log.Error(err);
                throw;
            }
        }

        private static string GetXCOMRootDirectoryFromVDFConfig()
        {
            var configPath = SteamUtilities.GetSteamConfigVDFPath();
            Log.Debug("Attempting to determine XCOM Root Directory from Config VDF [ {0} ]", configPath);

            var lines = File.ReadAllLines(configPath).Where(s => s.Contains(Common.XCOMInstallFolderName)).ToList();
            Log.Debug("Found Entries in VDF:\n{0}", string.Join("\n>", lines));

            if (lines.Count != 1)
            {
                throw new InvalidOperationException("Cannot Determine XCOM Root Directory from Config.VDF");
            }

            // format should be similar to: 
            // 						"installdir"		"X:\\Steam\\steamapps\\common\\XCom-Enemy-Unknown"

            var regex = new Regex(@"[\""]installdir[\""]\s+[\""](?<installdir>.+)[\""]", RegexOptions.IgnoreCase);
            var match = regex.Match(lines.First());

            var rootDirectory = match.Groups["installdir"].Value;
            Log.Debug("Regex Matched Install Directory of [ {0} ]", rootDirectory);
            return rootDirectory;
        }

        /// <summary>
        /// Decompresses a UPK File, by invoking the Unreal Engine Decompressor (decompress.exe)
        /// </summary>
        /// <param name="upkOutputDirectory">the output directory</param>
        /// <param name="targetFile">The file to decompress</param>
        /// <returns>the string to the decompressed file</returns>
        public static string DecompressUPKFile(string upkOutputDirectory, string targetFile)
        {
            var upkFileName = Path.GetFileName(targetFile);
            if (string.IsNullOrEmpty(upkFileName))
            {
                throw new InvalidOperationException(string.Format("Target File [ {0} ] is Invalid to Decompress", targetFile));
            }

            var arguments = string.Format("-out={0} {1}", upkOutputDirectory, targetFile);
            Log.Debug("Decompress Arguments [ {0} ]", arguments);

            var process = new Process()
            {
                StartInfo =
                {
                    FileName = Common.UPKDecompressorFileName, 
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };
            process.Start();
            process.WaitForExit();
            
            Log.Debug("Decompress Log:\n{0}", process.StandardOutput.ReadToEnd());
            Log.Debug("Decompress exited with code [ {0} ]", process.ExitCode);
            if (process.ExitCode < 0)
            {
                Log.Error("Error occured in decompress.exe\n{0}", process.StandardOutput.ReadToEnd());
                throw new InvalidOperationException(string.Format("Cannot unpack UPK file [ {0} ]", targetFile));
            }

            var decompressedUpkFilePath = Path.Combine(upkOutputDirectory, upkFileName);
            Log.Info("Decompressed Upk File [ {0} ]", decompressedUpkFilePath);
            return decompressedUpkFilePath;
        }

    }
}