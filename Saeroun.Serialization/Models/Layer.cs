using System.Xml.Serialization;

namespace Saeroun.Serialization.Models;

public class Layer
{
    [XmlAttribute("type")]
    public int Type;

    [XmlAttribute("height")]
    public int Height;

    [XmlAttribute("pos")]
    public int Position;
}