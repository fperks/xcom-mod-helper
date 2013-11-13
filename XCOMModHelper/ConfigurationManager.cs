using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XCOMModHelper
{
    public class ConfigurationManager
    {        
        public List<Tuple<string, string>> Patches { get; set; }

        public ConfigurationManager()
        {
            Patches = new List<Tuple<string, string>>();
        }

        public void LoadConfiguration(string filePath)
        {
            var xdoc = XDocument.Load(filePath);
            var patches = from patch in xdoc.Descendants("Patch")
                select new
                {
                    Find = (String)patch.Element("Find"),
                    Replace = (String)patch.Element("Replace")
                };

            foreach (var patch in patches)
            {
                var find = patch.Find.Trim();
                var replace = patch.Replace.Trim();

                Patches.Add(new Tuple<string, string>(find, replace));

                //Console.WriteLine("Find[{0}], Replace[{1}]", find, replace);
            }
        }
    }
}
