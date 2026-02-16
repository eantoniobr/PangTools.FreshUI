# Saeroun.Renderer

This library contains the renderer code to render Fresh UI
elements using ImageSharp.

## Usage

```csharp
using System.Xml.Serialization;
using Saeroun.Renderer;
using Saeroun.Serialization.DTO;
using Saeroun.Serialization.Factories;
using Saeroun.Serialization.Models;
using SixLabors.ImageSharp;

// Build a FileAtlas for the unpacked Pangya data/ folder
string[] files = Directory.GetFiles("C:/path/to/pangya/data", "*.*", SearchOption.AllDirectories);
FileAtlas fileAtlas = new(files);

// Define a XML serializer for Fresh UI XML data
XmlSerializer resourceSerializer = new(typeof(Resource));

// Load your target XML file and the frame resource file
FileStream elementFile = new FileStream(uiFile, FileMode.Open);
FileStream frameFile = new FileStream(framesFile, FileMode.Open);

// Deserialize the files into Resource classes
Resource resource = (Resource)resourceSerializer.Deserialize(elementFile);
Resource frameResource = (Resource)resourceSerializer.Deserialize(frameFile);

// Build a FrameInfoAtlas for the renderer using the frame Resource
FrameInfoAtlas frameInfoAtlas =
    FrameInfoFactory.BuildFrameInfo(frameResource.Elements);

// Create an instance of the renderer using the Frame and FileInfo atlasses
ImageSharpRenderer renderer = new(fileAtlas, frameInfoAtlas);

// Render ImageSharp image objects using .RenderAllElements
Dictionary<string, Image> images = renderer.RenderAllElements(resource, buttonState);

// Save the images to disk
foreach(KeyValuePair<string, Image> entry in images)
{
    entry.Value.Save(outputDirectory + entry.Key); 
}
```