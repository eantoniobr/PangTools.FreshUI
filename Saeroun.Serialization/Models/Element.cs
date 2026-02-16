using System.Xml.Serialization;

namespace Saeroun.Serialization.Models;

public class Element
{
    [XmlAttribute("type")]
    public string Type;
    
    [XmlAttribute("name")]
    public string Name;
    
    [XmlAttribute("size")]
    public string? Size;
    
    [XmlAttribute("resource")]
    public string? Resource;
    
    [XmlElement("item")]
    public List<Item>? Items;
    
    [XmlElement("layer")]
    public List<Layer>? Layers;
    
    [XmlElement("bfrm")]
    public Frame? BorderFrame;
    
    [XmlElement("sfrm")]
    public Frame? SquareFrame;
    
    [XmlElement("cfrm")]
    public Frame? ClearFrame;

    [XmlElement("base")] 
    public Base? Base;
}