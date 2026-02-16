using System.Xml.Serialization;

namespace Saeroun.Serialization.Models;

public class Frame
{
    [XmlAttribute("filename")]
    public string FileName;
}