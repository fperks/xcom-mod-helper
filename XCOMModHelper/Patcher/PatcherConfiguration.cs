using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using NLog;

namespace XCOMModHelper.Patcher
{
    [XmlRoot("PatcherConfiguration")]
    public class PatcherConfiguration
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        [XmlElement("BackupDirectory")]
        public string BackupDirectory { get; set; }

        [XmlElement("DecompressedUPKOutputDirectory")]
        public string DecompressedUPKOutputDirectory { get; set; }

        [XmlArray("PatchTargets")]
        [XmlArrayItem("PatchTarget")]
        public List<PatchTarget> Targets { get; set; }

        public PatcherConfiguration()
        {
            Targets = new List<PatchTarget>();
        }

        /// <summary>
        /// Deserializes a configuration file
        /// </summary>
        /// <param name="filepath">The path to the configuration file</param>
        /// <returns>A PatcherConfiguration object</returns>
        public static PatcherConfiguration LoadConfiguration(string filepath)
        {
            Log.Debug("Desrializing [ {0} ]", filepath);
            PatcherConfiguration config;
            var serializer = new XmlSerializer(typeof(PatcherConfiguration));
            using (var reader = XmlReader.Create(filepath))
            {
                try
                {
                    config = (PatcherConfiguration)serializer.Deserialize(reader);
                }
                catch (Exception)
                {
                    throw new InvalidOperationException(string.Format("File [ {0} ] is not a valid configuration file",
                        filepath));
                }
            }
            Log.Debug("Loaded Configuration File [ {0} ]", filepath);
            return config;
        }

        /// <summary>
        /// Writes the PatcherConfiguration file to the specified file path
        /// </summary>
        /// <param name="config">The PatcherConfiguration object to be serialized</param>
        /// <param name="filepath">The output filepath</param>
        public static void WriteConfiguration(PatcherConfiguration config, string filepath)
        {
            Log.Debug("Serializing Configuration to [ {0} ]", filepath);
            var serializer = new XmlSerializer(typeof(PatcherConfiguration));
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
            };

            using (var writer = XmlWriter.Create(filepath, settings))
            {
                serializer.Serialize(writer, config);
            }
            Log.Debug("Configuration File was written successfully to [ {0} ]", filepath);
        }
    }
}