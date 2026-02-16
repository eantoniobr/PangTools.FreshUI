namespace Saeroun.Serialization.Models;

using System.Xml.Serialization;

[XmlRoot("resource")]
public class Resource
{
    [XmlAttribute("count")]
    public int Count;

    [XmlElement("element")]
    public List<Element> Elements;
}