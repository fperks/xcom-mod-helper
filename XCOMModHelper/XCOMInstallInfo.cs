namespace XCOMModHelper
{
    public class XCOMInstallInfo
    {
        public XCOMProductType InstallType { get; private set; }

        public string XCOMBinaryDirectory { get; set; }

        public string XCOMGameInstallDirectory { get; set; }

        public string XCOMExecutablePath { get; set; }

        public XCOMInstallInfo(XCOMProductType installType)
        {
            InstallType = installType;
        }

        public override string ToString()
        {
            return
                string.Format(
                    "Install Type: {0}\nGame Install Directory: {1}\nExecutablePath: {2}\nBinary Directory{3}",
                    InstallType, XCOMGameInstallDirectory, XCOMExecutablePath, XCOMBinaryDirectory);
        }
    }
}