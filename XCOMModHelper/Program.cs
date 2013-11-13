using System;
using System.IO;

using CommandLine;

using NLog;
using NLog.Config;
using NLog.Targets;

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

            var configFile = options.ConfigurationFile;
            if (!File.Exists(configFile))
            {
                Log.Error("Configuration File [ {0} ] does not exist", configFile);
                return -1;
            }
            
            var gameType = options.XCOMGameType.ToUpper();
            if (!Common.XCOMProductTypeMapper.ContainsKey(gameType))
            {
                Log.Error("Game Type of [ {0} ] is Invalid, Valid types are: {1}", gameType, string.Join(",", Common.XCOMProductTypeMapper.Keys));
                return -1;
            }
            
            if (!string.IsNullOrEmpty(options.XCOMInstallDirectory))
            {
                if (!Directory.Exists(options.XCOMInstallDirectory))
                {
                    Log.Error("The Specified XCOM Install Directory [ {0} ] is not valid", options.XCOMInstallDirectory);
                    return -1;
                }
            }
            
            var productType = Common.XCOMProductTypeMapper[gameType];
            var modHelper = new XCOMModHelper
            {
                ConfigurationFile = options.ConfigurationFile,
                IsVerbose = options.Verbose,
                ProductType = productType,
                InstallManager = {XCOMInstallDirectory = options.XCOMInstallDirectory},
            };

            Log.Info("==========XCOMModHelper========");

            try
            {
                modHelper.Execute();
            }
            catch (Exception err)
            {
                Log.Error("An Error occured during processing:\n");
                Log.Error(err);
                Log.Error("\nThe Patching Process was Aborted");
                return -1;
            }
            Log.Info("Finished");
            return 0;
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
