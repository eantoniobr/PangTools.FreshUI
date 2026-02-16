using System.Xml.Serialization;

namespace Saeroun.Serialization.Models;

public class Item
{
    [XmlAttribute("type")]
    public string Type;
    
    [XmlAttribute("flag")]
    public int Flag;
    
    [XmlAttribute("name")]
    public string Name;
    
    [XmlAttribute("caption")]
    public string? Caption;

    [XmlAttribute("resource")] 
    public string? Resource;
    
    [XmlAttribute("pos")]
    public string? Position;
    
    [XmlAttribute("rect")]
    public string? Rectangle;

    [XmlElement("param")]
    public List<Parameter>? Parameters;
    
    public Parameter? GetParameter(string parameterName)
    {
        return Parameters.FirstOrDefault(p => p.Name.Equals(parameterName));
    }
    
    [XmlElement("item")]
    public List<Item>? Items;
}