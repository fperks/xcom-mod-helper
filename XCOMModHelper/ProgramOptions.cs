using CommandLine;
using CommandLine.Text;

namespace XCOMModHelper
{
    class ProgramOptions
    {
        //[Option('g', "gametype", Required = true, HelpText = "Specify the XCOM Game Type to Patch Valid Types are EU (Enemy Unknown) or EW (Enemy Within)")]
        //public string XCOMGameType { get; set; }

        [Option('c', "config", Required = true, HelpText = "The required configuration file")]
        public string ConfigurationFile { get; set; }

        [Option('v', "verbose", DefaultValue = false, HelpText = "Prints Detailed Output")]
        public bool Verbose { get; set; }

        [Option('t', "test", DefaultValue = false, HelpText = "Attempts to apply the patches but does not write the changes")]
        public bool IsTest { get; set; }

        [Option('x', "xcomdir", HelpText = "Specify the Path to XCOM Root Install Directory, this is optional and will be automatically determined")]
        public string UserDefinedXCOMRootDirectory { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var helpText = new HelpText
            {
                Heading = new HeadingInfo("XCOMModHelper"),
                AddDashesToOption = true,
                Copyright = new CopyrightInfo("Frank Perks", 2013),
                AdditionalNewLineAfterOption = true
            };

            helpText.AddPreOptionsLine("Usage: XCOMModHelper -c <config file>.xml");
            helpText.AddOptions(this);
            return helpText;
        }
    }
}
