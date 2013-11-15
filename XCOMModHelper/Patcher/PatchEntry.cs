using System.Xml.Serialization;

namespace XCOMModHelper.Patcher
{
    public class PatchEntry
    {
        [XmlElement("Description")]
        public string Description { get; set; }

        [XmlElement("FindValue")]
        public string FindValue { get; set; }

        [XmlElement("ReplaceValue")]
        public string ReplaceValue { get; set; }

        public override string ToString()
        {
            return string.Format("Description={0}, FindValue={1}, ReplaceValue={2}", Description, FindValue,
                ReplaceValue);
        }
    }
}