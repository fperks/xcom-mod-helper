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

        public const string UPKDecompressorFileName = @"decompress.exe";
    }
}
