using PangTools.FreshUI.Renderer.Extensions;
using PangTools.FreshUI.Serialization.DTO;
using PangTools.FreshUI.Serialization.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace PangTools.FreshUI.Renderer;

public class ImageSharpRenderer
{
    private bool _debug;

    private FileAtlas _fileAtlas;

    private FrameInfoAtlas _frameInfoAtlas;
    
    public ImageSharpRenderer(FileAtlas fileAtlas, FrameInfoAtlas frameInfoAtlas, bool debug)
    {
        _fileAtlas = fileAtlas;
        _frameInfoAtlas = frameInfoAtlas;
        _debug = debug;
    }

    public Dictionary<string, Image> RenderAllElements(Resource resource, string buttonState)
    {
        Dictionary<string, Image> images = new();

        foreach (Element element in resource.Elements)
        {
            Image image = new Image<Rgba32>(1024, 768, Color.Transparent);

            switch (element.Type)
            {
                case "FORM":
                case "LAYOUT":
                case "MACROITEM":
                    RenderElement(ref image, element, buttonState);
                    break;
            }
            
            image.Mutate(ctx => CropImage(ref ctx, element));
            
            images.Add($"{element.Name}_{buttonState}.png", image);
        }

        return images;
    }

    private Image RenderElement(ref Image image, Element element, string buttonState)
    {
        image.Mutate(ctx =>
        {
            switch (element.Type)
            {
                case "FORM":
                    ctx.DrawElementFrame(element, _fileAtlas, _frameInfoAtlas);
                    break;
            }

            if (element.Base != null)
            {
                ctx.DrawBase(element.Base, _fileAtlas);
            }

            if (element.Items?.Count > 0)
            {
                RenderItems(ref ctx, element.Items, buttonState);    
            }
        });

        return image;
    }

    private void RenderItems(ref IImageProcessingContext ctx, List<Item> items, string buttonState)
    {
        Color debugColor = Color.Transparent;
        
        foreach (Item item in items)
        {
            switch (item.Type)
            {
                case "AREA":
                    ctx.DrawArea(item, _fileAtlas);
                    debugColor = Color.Blue;
                    break;
                case "BUTTON":
                    ctx.DrawButton(item, buttonState, _fileAtlas);
                    debugColor = Color.Green;
                    break;
                case "COMBOCTLEX":
                    // NOTE: Reusing button rendering here because it's effectively
                    //       a fancy button in viewing perspective anyway.
                    ctx.DrawButton(item, buttonState, _fileAtlas);
                    debugColor = Color.SpringGreen;
                    break;
                case "FRAME":
                    ctx.DrawItemFrame(item, _fileAtlas, _frameInfoAtlas);
                    debugColor = Color.Red;
                    break;
                case "STATIC":
                    ctx.DrawStatic(item);
                    debugColor = Color.Pink;
                    break;
                case "GROUPBOX":
                    if (item.Items?.Count > 0)
                    {
                        RenderItems(ref ctx, item.Items, buttonState);    
                    }
                    debugColor = Color.BlueViolet;
                    break;
                case "EDIT":
                case "COMBOBOX":
                    ctx.DrawInput(item);
                    debugColor = Color.Coral;
                    break;
                case "GAUGEBAR":
                case "GAUGEBAREX":
                    ctx.DrawGaugebar(item);
                    debugColor = Color.SlateGray;
                    break;
                case "GAUGEBARIMAGE":
                    // NOTE: Reusing button rendering here as just a
                    //       "normal" parameter image is rendered here.
                    //
                    // TODO: Add own method, support overlay masking
                    ctx.DrawButton(item, "normal", _fileAtlas);
                    debugColor = Color.DarkSlateGray;
                    break;
                case "LISTBOX":
                    // Listboxes don't tell us what they render
                    // So they just exist in debug output
                    debugColor = Color.Magenta;
                    break;
                case "TABBUTTON":
                    // Tab Buttons only define hover/selected states
                    // So in default renderings they aren't even visible
                    // TODO: Add rendering support for the other states
                    debugColor = Color.DarkOrange;
                    break;
                case "TEXTBUTTON":
                    ctx.DrawTextButton(item);
                    debugColor = Color.MediumSeaGreen;
                    break;
                case "VIEWER":
                    ctx.DrawViewer(item, _fileAtlas);
                    debugColor = Color.Purple;
                    break;
            }
            
            if (_debug)
            {
                ctx.DrawDebugHint(item, debugColor, _fileAtlas);
                
                // Reset debug color to be transparent for non-supported elements
                debugColor = Color.Transparent;
            }
        }
    }

    private void CropImage(ref IImageProcessingContext ctx, Element element)
    {
        if (element.Size != null)
        {
            int[] sizes = element.Size.Split(" ").Select(Int32.Parse).ToArray();
            int width = sizes[0];
            int height = sizes[1];
                
            ctx.Crop(new Rectangle(0, 0, width, height));
        }
        else if (element.Base != null)
        {
            ctx.Crop(new Rectangle(0, 0, (int)element.Base.Width, (int)element.Base.Height));
        }
    }
}