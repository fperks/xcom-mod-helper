using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

using CommandLine;
using CommandLine.Text;
using System.IO;

namespace XCOMEUModHelper
{
    class Program
    {
        class Options
        {
            [Option('c', "config", Required = true, HelpText = "The required configuration file")]
            public string ConfigurationFile { get; set; }

            [Option('v', "verbose", DefaultValue = false, HelpText = "Prints Detailed Output")]
            public bool Verbose { get; set; }

            [Option('x', "xcomdir", HelpText = "Specify the Path to XCOM Root Directory, this is optional and will be automatically determined")]
            public string XCOMDirectory { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                var helpText = new HelpText
                {
                    Heading = new HeadingInfo("XCOMEUModHelper"),
                    AddDashesToOption = true,
                    Copyright = new CopyrightInfo("Frank Perks", 2013),
                    AdditionalNewLineAfterOption = true
                };
                
                helpText.AddPreOptionsLine("Usage: XCOMEUModHelper -c <config file>.xml");
                helpText.AddOptions(this);
                return helpText;
            }
        }


        static int Main(string[] args)
        {
            var options = new Options();
            if (!Parser.Default.ParseArguments(args, options))
            {
                //return;
            }
            Console.WriteLine("\n");
            if (!string.IsNullOrEmpty(options.XCOMDirectory))
            {
                if (!Directory.Exists(options.XCOMDirectory))
                {
                    Console.WriteLine("[ERROR] The specified executable path [ {0} ] does not exist", options.XCOMDirectory);
                    return -1;
                }                
            }
            else
            {
                // Otherwise just try and find the current one
                options.XCOMDirectory = Helpers.GetXCOMRootDirectory();    
            }

            if (!File.Exists(options.ConfigurationFile))
            {
                Console.WriteLine("[ERROR] The Specified Configuration File [ {0} ] does not exist", options.ConfigurationFile);
                return -1;
            }

            var exectuablePath = Helpers.GetXCOMExecutablePath(options.XCOMDirectory);
            var config = new ConfigurationManager();
            config.LoadConfiguration(options.ConfigurationFile);
            var patcher = new ExecutablePatcher(exectuablePath);

            BackupXCOMExecutable(exectuablePath);

            Console.WriteLine("Attempting to Patch XCOM Executable {0}", exectuablePath);

            // Apply all of the patches to the executable
            foreach (var patch in config.Patches)
            {
                var find = patch.Item1;
                var replace = patch.Item2;
                var findBytes = find.GetByteArrayFromHexString();
                var replaceBytes = replace.GetByteArrayFromHexString();

                Console.WriteLine("Patching\nFind> {0}\nReplace>{1}", find, replace);

                //TestConversion(find, replace);

                if (options.Verbose)
                {
                    Console.WriteLine("HF> {0}", PadHexString(find));
                    Console.WriteLine("HR> {0}", PadHexString(replace));
                    Console.WriteLine("BF> {0}", string.Join(" ", findBytes.Select(b => String.Format("{0,3}", b.ToString(CultureInfo.InvariantCulture)))));
                    Console.WriteLine("BR> {0}", string.Join(" ", replaceBytes.Select(b => String.Format("{0,3}", b.ToString(CultureInfo.InvariantCulture)))));
                }

                try
                {
                    patcher.ApplyPatch(findBytes, replaceBytes);
                }
                catch (Exception err)
                {
                    Console.WriteLine("Error Occured While Attempting to XCOM Exectuable\n{0}", err);
                    return -1;
                }                
            }

            Console.WriteLine("Saving Changes to XCOM Exectuable");
            patcher.SaveChanges();

            Console.WriteLine();
            
            return 0;
        }

        public static void BackupXCOMExecutable(string exePath)
        {
            var binDir = Path.GetDirectoryName(exePath);
            if (string.IsNullOrEmpty(binDir))
            {
                throw new InvalidOperationException("Cannot Retrieve the XCOM Binary Directory");
            }

            var backupPath = Path.Combine(binDir, "XCOMGame-backup.exe");
            Console.WriteLine("Backing up XCOM Executable to [ {0} ] ", backupPath);
            File.Copy(exePath, backupPath, true);
        }

        public static void TestConversion(string find, string replace)
        {
            Console.WriteLine("HF> {0}", PadHexString(find));
            Console.WriteLine("HR> {0}", PadHexString(replace));

            var findBytes = find.GetByteArrayFromHexString();
            var replaceBytes = replace.GetByteArrayFromHexString();

            Console.WriteLine("BF> {0}", string.Join(" ", findBytes.Select(b => String.Format("{0,3}", b.ToString(CultureInfo.InvariantCulture)))));
            Console.WriteLine("BR> {0}", string.Join(" ", replaceBytes.Select(b => String.Format("{0,3}", b.ToString(CultureInfo.InvariantCulture)))));
        }

        public static string PadHexString(string source)
        {
            var tokens = source.Split(new[] {' '});
            
            // pad with leading zero
            return string.Join(" ", tokens.Select(s => string.Format("{0,3}", s)));
        }
    }
}
