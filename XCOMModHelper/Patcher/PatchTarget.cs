using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace XCOMModHelper.Patcher
{
    public class PatchTarget
    {
        [XmlElement("TargetPath")]
        public string TargetPath { get; set; }

        [XmlElement("IsUPKFile")]
        public bool IsUPKFile { get; set; }

        [XmlArray("Patches")]
        [XmlArrayItem("PatchEntry")]
        public List<PatchEntry> PatchEntries { get; set; }

        public PatchTarget()
        {
            PatchEntries = new List<PatchEntry>();
        }

        public string GetFullTargetPath(string rootDirectory)
        {
            return Path.Combine(rootDirectory, TargetPath);
        }

        public override string ToString()
        {
            return string.Format("(TargetPath={0}, IsUpkFile={1}, \nEntries={2}", TargetPath, IsUPKFile,
                string.Join("\n", PatchEntries.Select(x => string.Format("({0})", x.ToString()))));
        }
    }
}