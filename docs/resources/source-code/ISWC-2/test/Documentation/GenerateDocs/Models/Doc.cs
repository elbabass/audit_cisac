using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace SpanishPoint.Azure.Iswc.Api.Tests.Documentation.GenerateDocs.Models
{
    [ExcludeFromCodeCoverage]
    [XmlRoot(ElementName = "assembly")]
    public class Assembly
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
    }

    [ExcludeFromCodeCoverage]
    [XmlRoot(ElementName = "member")]
    public class Member
    {
        [XmlElement(ElementName = "summary")]
        public string Summary { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [ExcludeFromCodeCoverage]
    [XmlRoot(ElementName = "members")]
    public class Members
    {
        [XmlElement(ElementName = "member")]
        public List<Member> Member { get; set; }
    }

    [ExcludeFromCodeCoverage]
    [XmlRoot(ElementName = "doc")]
    public class Doc
    {
        [XmlElement(ElementName = "assembly")]
        public Assembly Assembly { get; set; }
        [XmlElement(ElementName = "members")]
        public Members Members { get; set; }
    }
}