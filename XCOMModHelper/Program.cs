using System;
using System.IO;

using CommandLine;

using NLog;
using NLog.Config;
using NLog.Targets;
using XCOMModHelper.Patcher;

namespace XCOMModHelper
{
    class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        static int Main(string[] args)
        {
            var options = new ProgramOptions();
            if (!Parser.Default.ParseArguments(args, options))
            {
                return -1;
            }

            InitializeLogging(options.Verbose);

            TestSerialization();

            var configFile = options.ConfigurationFile;
            if (!File.Exists(configFile))
            {
                Log.Error("Configuration File [ {0} ] does not exist", configFile);
                return -1;
            }
            
            if (!string.IsNullOrEmpty(options.UserDefinedXCOMRootDirectory))
            {
                if (!Directory.Exists(options.UserDefinedXCOMRootDirectory))
                {
                    Log.Error("The Specified XCOM Root Directory [ {0} ] is not valid", options.UserDefinedXCOMRootDirectory);
                    return -1;
                }
            }
            
            var modHelper = new XCOMModHelper
            {
                ConfigurationFile = options.ConfigurationFile,
                IsVerbose = options.Verbose,
                IsPatchTest = options.IsTest,
                XCOMRootDirectory = options.UserDefinedXCOMRootDirectory
            };

            Log.Info("==========XCOMModHelper========");

            //modHelper.Execute();

            try
            {
                modHelper.Execute();
            }
            catch (Exception err)
            {
                Log.Error("An Error occured during processing:{0}", err.Message);
                //Log.Error(err);
                Log.Error("\nThe Patching Process was Aborted");
                return -1;
            }
            Log.Info("Finished");
            return 0;
        }

        private static void TestSerialization()
        {
            var config = new PatcherConfiguration()
            {
                BackupDirectory = @"XEW\Backup",
                DecompressedUPKOutputDirectory = @"XEW\UpkUnpacked"
            };

            config.Targets.Add(new PatchTarget()
            {
                TargetPath = @"XEW\Binaries\Win32\XComEW.exe",
                IsUPKFile = false,
                PatchEntries =
                {
                    new PatchEntry()
                    {
                        Description = "Read DefaultGameCore.ini from Config Folder",
                        FindValue =
                            "   25 00 64 00 00 00 00 00 49 00 6e 00 69 00 56 00 65 00 72 00 73 00 69 00 6f 00 6e 00 00 00 00 00 2e 00 2e 00 5c 00 2e 00 2e 00 5c 00 58 00 43 00 ",
                        ReplaceValue =
                            "   25 00 64 00 00 00 00 00 49 00 6e 00 69 00 56 00 65 00 72 00 73 00 69 00 6f 00 6e 00 00 00 00 00 2e 00 2e 00 5c 00 2e 00 2e 00 5c 00 57 00 43 00 "
                    }
                }
            });

            config.Targets.Add(new PatchTarget()
            {
                IsUPKFile = true,
                TargetPath = @"XEW\XComGame\CookedPCConsole\XComGame.upk",
                PatchEntries =
                {
                    new PatchEntry()
                    {
                        Description = "Changes Gender Chance from 1/2 chance of female, to 1/8 chance of female",
                        FindValue = "45 9A A7 2C 02 16",
                        ReplaceValue = "45 9A A7 2C 08 16"
                    }
                }                
            });

            PatcherConfiguration.WriteConfiguration(config, "test_config.xml");
        }

        private static void InitializeLogging(bool isVerbose)
        {
            var config = new LoggingConfiguration();
            var consoleTarget = new ConsoleTarget();
            var fileTarget = new FileTarget();

            consoleTarget.Layout = Common.LogFormat;
            fileTarget.Layout = Common.LogFormat;
            fileTarget.FileName = Common.LogFileName;
            

            if (isVerbose)
            {
                config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, consoleTarget));
                config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, fileTarget));
            }
            else
            {
                config.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, consoleTarget));
                config.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, fileTarget));
            }
            LogManager.Configuration = config;
        }

    }
}
