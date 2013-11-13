using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XCOMModHelper
{
    public static class Common
    {
        public const string LogFormat = @"[${level:uppercase=true}]> ${message}";
        public const string LogFileName = "log.txt";

        public const string XCOMSteamAppsPath = @"SteamApps\common\XCom-Enemy-Unknown";
        public const string XCOMInstallFolderName = "XCom-Enemy-Unknown";

        public const string DefaultXCOMBinaryDirectory = @"Binaries\Win32";
        public const string DefaultXCOMEUExecutableName = @"XComGame.exe";
        public const string DefaultXCOMEUBackupExecutableName = @"(Backup)XComGame.exe";

        public const string DefaultXCOMEWInstallDirectoryName = @"XEW";
        public const string DefaultXCOMEWExecutableName = @"XComEW.exe";
        public const string DefaultXCOMEWBackupExecutableName = @"bXComEW.exe";

        public static Dictionary<string, XCOMProductType> XCOMProductTypeMapper;

        static Common()
        {
            XCOMProductTypeMapper = new Dictionary<string, XCOMProductType>()
            {
                {"EW", XCOMProductType.XCOMEnemyWithin},
                {"EU", XCOMProductType.XCOMEnemyUnknown}
            };
        }

    }
}
