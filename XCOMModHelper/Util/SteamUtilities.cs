using System;
using System.IO;
using Microsoft.Win32;
using NLog;

namespace XCOMModHelper.Util
{
    public static class SteamUtilities
    {
        public static Logger Logger = LogManager.GetCurrentClassLogger();

        public const string RegistryKeySteamPathName = @"HKEY_CURRENT_USER\Software\Valve\Steam";
        public const string RegistryValueSteamPath = "SteamPath";

        public const string DefaultConfigVDFPath = @"config/config.vdf";
        /// <summary>
        /// Determine the Steam root directory
        /// </summary>
        /// <returns>A string containing the path to the Steam Root directory</returns>
        public static string GetSteamRootDirectory()
        {
            var steamPath = Registry.GetValue(RegistryKeySteamPathName, RegistryValueSteamPath, string.Empty) as string;
            if (string.IsNullOrEmpty(steamPath))
            {
                throw new InvalidOperationException("Cannot Find the Steam Direcotry");
            }

            if (!Directory.Exists(steamPath))
            {
                throw new InvalidOperationException(string.Format("The Steam Directory specified [ {0} ] is not valid or accessible"));
            }
            return steamPath;
        }

        public static string GetSteamConfigVDFPath()
        {
            var result = Path.Combine(GetSteamRootDirectory(), DefaultConfigVDFPath);
            Logger.Debug("Found Config.VDF path [ {0} ]", result);
            
            // Make sure it exists and is accessible
            if (!File.Exists(result))
            {
                throw new InvalidOperationException(string.Format("Config.vdf file is not accessible [ {0} ]", result));
            }
            return result;
        }

        
    }
}
