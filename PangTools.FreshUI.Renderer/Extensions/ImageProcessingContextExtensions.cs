using System.Numerics;
using PangTools.FreshUI.Renderer.Helpers;
using PangTools.FreshUI.Serialization.DTO;
using SixLabors.ImageSharp;
using PangTools.FreshUI.Serialization.Models;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;

namespace PangTools.FreshUI.Renderer.Extensions;

public static class ImageProcessingContextExtensions
{
    public static IImageProcessingContext DrawElementFrame(this IImageProcessingContext ctx, Element element, FileAtlas fileAtlas, FrameInfoAtlas frameInfoAtlas)
    {
        Size? size = DimensionHelper.ParseSize(element.Size);

        ctx.DrawFrame(element.Resource, new Rectangle(0, 0, size.Value.Width, size.Value.Height), fileAtlas, frameInfoAtlas);

        return ctx;
    }

    public static IImageProcessingContext DrawItemFrame(this IImageProcessingContext ctx, Item frameItem,
        FileAtlas fileAtlas, FrameInfoAtlas frameInfoAtlas)
    {
        RectangleF? frameRect = DimensionHelper.ParseRectangle(frameItem.Rectangle);
        Parameter? bgImgParam = frameItem.GetParameter("bgimg");

        if (frameRect != null)
        {
            ctx.DrawFrame(frameItem.Resource, (Rectangle)frameRect, fileAtlas, frameInfoAtlas, bgImgParam?.Value, bgImgParam?.Value);
        }

        return ctx;
    }
    
    public static IImageProcessingContext DrawArea(this IImageProcessingContext ctx, Item areaItem, FileAtlas fileAtlas)
    {
        RectangleF? areaItemRect = DimensionHelper.ParseRectangle(areaItem.Rectangle);

        if (areaItemRect == null)
        {
            return ctx;
        }

        Parameter? bgParam = areaItem.GetParameter("bgimg");
        Parameter? stretchParam = areaItem.GetParameter("stretch");
        Parameter? visibleParam = areaItem.GetParameter("visible");

        if (visibleParam != null && visibleParam.Value == "0")
        {
            return ctx;
        }

        if (bgParam != null && bgParam.Value.Length > 0)
        {
            Image? bgTexture = fileAtlas.GetImage($"{bgParam.Value}.");

            if (bgTexture != null)
            {
                if (stretchParam != null && stretchParam.Value == "1")
                {
                    bgTexture.Mutate(bgCtx =>
                    {
                        bgCtx.Resize(new Size(
                            (int)areaItemRect.Value.Width,
                            (int)areaItemRect.Value.Height
                        ));
                    });
                }
                
                ctx.DrawImage(bgTexture, new Point((int)areaItemRect.Value.X, (int)areaItemRect.Value.Y), 1f); 
            }
        }
        
        return ctx;
    }

    public static IImageProcessingContext DrawButton(this IImageProcessingContext ctx, Item buttonItem, string buttonState, FileAtlas fileAtlas)
    {
        int[]? pos = null;
        
        if (buttonItem.Position != null)
        {
            pos = DimensionHelper.TextToIntArray(buttonItem.Position);
        }

        if (buttonItem.Rectangle != null)
        {
            int[] buttonRect = DimensionHelper.TextToIntArray(buttonItem.Rectangle);

            pos = new int[2]
            {
                buttonRect[0],
                buttonRect[1]
            };
        }
        
        Parameter? stateParam = buttonItem.GetParameter(buttonState);

        if (pos != null && stateParam != null && stateParam.Value.Length > 0)
        {
            Image? normalTexture = fileAtlas.GetImage($"{stateParam.Value}.");

            if (normalTexture != null)
            {
                ctx.DrawImage(normalTexture, new Point(pos[0], pos[1]), 1f);    
            }
        }

        return ctx;
    }

    public static IImageProcessingContext DrawFrame(this IImageProcessingContext ctx, string resource, Rectangle rect, FileAtlas fileAtlas, FrameInfoAtlas frameInfoAtlas, string frameType = "bfrm", string? frameImage = null)
    {
        if (!frameInfoAtlas.ContainsKey(resource))
        {
            return ctx;
        }
        
        Dictionary<string, FrameInfo> frameInfo = frameInfoAtlas[resource];
        if (frameInfo.ContainsKey(frameType ?? "") || frameImage != null)
        {
            FrameInfo borderFrameInfo = null;
            if (frameInfo.ContainsKey(frameType ?? ""))
            {
                borderFrameInfo = frameInfo[frameType];    
            }

            Image? topLeftImage = frameImage != null 
                ? fileAtlas.GetImage($"{frameImage}00")
                : fileAtlas.GetImage(borderFrameInfo.FileNames[0]);
            Image? topCenterImage = frameImage != null 
                ? fileAtlas.GetImage($"{frameImage}01")
                : fileAtlas.GetImage(borderFrameInfo.FileNames[1]);
            Image? topRightImage = frameImage != null 
                ? fileAtlas.GetImage($"{frameImage}02")
                : fileAtlas.GetImage(borderFrameInfo.FileNames[2]);
            Image? centerLeftImage = frameImage != null 
                ? fileAtlas.GetImage($"{frameImage}03")
                : fileAtlas.GetImage(borderFrameInfo.FileNames[3]);
            Image? centerImage = frameImage != null 
                ? fileAtlas.GetImage($"{frameImage}04") 
                : fileAtlas.GetImage(borderFrameInfo.FileNames[4]);
            Image? centerRightImage = frameImage != null 
                ? fileAtlas.GetImage($"{frameImage}05") 
                : fileAtlas.GetImage(borderFrameInfo.FileNames[5]);
            Image? bottomLeftImage = frameImage != null 
                ? fileAtlas.GetImage($"{frameImage}06") 
                : fileAtlas.GetImage(borderFrameInfo.FileNames[6]);
            Image? bottomCenterImage = frameImage != null 
                ? fileAtlas.GetImage($"{frameImage}07") 
                : fileAtlas.GetImage(borderFrameInfo.FileNames[7]);
            Image? bottomRightImage = frameImage != null 
                ? fileAtlas.GetImage($"{frameImage}08") 
                : fileAtlas.GetImage(borderFrameInfo.FileNames[8]);
            
            if (topLeftImage != null)
            {
                ctx.DrawImage(topLeftImage, new Point(rect.X, rect.Y), 1f);
            }

            if (topCenterImage != null)
            {
                topCenterImage.Mutate(imgCtx => imgCtx.Resize(new Size(
                        rect.Width - topLeftImage.Width - topRightImage.Width,
                        topCenterImage.Height
                    )));

                ctx.DrawImage(topCenterImage, new Point(rect.X + topLeftImage.Width, rect.Y), 1f);
            }
            
            if (topRightImage != null)
            {
                ctx.DrawImage(topRightImage, new Point(rect.X + rect.Width - topRightImage.Width, rect.Y), 1f);
            }
            
            if (centerLeftImage != null)
            {
                centerLeftImage.Mutate(imgCtx => imgCtx.Resize(new Size(
                    centerLeftImage.Width,
                    rect.Height - topLeftImage.Height - bottomLeftImage.Height
                )));

                ctx.DrawImage(centerLeftImage, new Point(rect.X, rect.Y + topLeftImage.Height), 1f);
            }

            if (centerImage != null)
            {
                centerImage.Mutate(imgCtx => imgCtx.Resize(new Size(
                    rect.Width - centerLeftImage.Width - centerRightImage.Width,
                    rect.Height - topLeftImage.Height - bottomLeftImage.Height
                )));

                ctx.DrawImage(centerImage, new Point(rect.X + topLeftImage.Width, rect.Y + topLeftImage.Height), 1f);
            }
            
            if (centerRightImage != null)
            {
                centerRightImage.Mutate(imgCtx => imgCtx.Resize(new Size(
                    centerRightImage.Width,
                    rect.Height - topRightImage.Height - bottomRightImage.Height
                )));

                ctx.DrawImage(centerRightImage, new Point(rect.X + rect.Width - centerRightImage.Width, rect.Y + topRightImage.Height), 1f);
            }
            
            if (bottomLeftImage != null)
            {
                ctx.DrawImage(bottomLeftImage, new Point(rect.X, rect.Y + rect.Height - bottomLeftImage.Height), 1f);
            }
            
            if (bottomCenterImage != null)
            {
                bottomCenterImage.Mutate(imgCtx => imgCtx.Resize(new Size(
                    rect.Width - bottomLeftImage.Width - bottomRightImage.Width,
                    bottomCenterImage.Height
                )));

                ctx.DrawImage(bottomCenterImage, new Point(rect.X + bottomLeftImage.Width, rect.Y + rect.Height - bottomCenterImage.Height), 1f);
            }
            
            if (bottomRightImage != null)
            {
                ctx.DrawImage(bottomRightImage, new Point(rect.X + rect.Width - bottomRightImage.Width, rect.Y + rect.Height - bottomRightImage.Height), 1f);
            }
        }

        return ctx;
    }

    public static IImageProcessingContext DrawStatic(this IImageProcessingContext ctx, Item staticItem)
    {
        int[] pos = DimensionHelper.TextToIntArray(staticItem.Position);

        if (pos != null && staticItem.Caption != null)
        {
            Parameter? colorParam = staticItem.GetParameter("color");
            Parameter? alignParam = staticItem.GetParameter("align");
            Color textColor;
            HorizontalAlignment horizontalAlignment;
            
            if (colorParam != null)
            {
                string staticColor = ColorHelper.ARGBtoRBGA(colorParam.Value);
                textColor = Color.Parse(staticColor);
            }
            else
            {
                textColor = Color.Black;
            }

            if (alignParam != null)
            {
                switch (int.Parse(alignParam.Value))
                {
                    case 1:
                        horizontalAlignment = HorizontalAlignment.Center;
                        break;
                    case 2:
                        horizontalAlignment = HorizontalAlignment.Right;
                        break;
                    default:
                        horizontalAlignment = HorizontalAlignment.Left;
                        break;
                }
            }
            else
            {
                horizontalAlignment = HorizontalAlignment.Left;
            }
            
            // TODO: Either pick a font closely referencing Pangya's UI
            //       Or add support for laoding in WFT font files
            FontFamily arialFontFamily = SystemFonts.Get("Malgun Gothic");
            Font arial = arialFontFamily.CreateFont(14f, FontStyle.Regular);
            
            ctx.DrawText(
                new DrawingOptions()
                {
                    Transform = Matrix3x2.CreateTranslation(pos[0], pos[1])
                },
                new RichTextOptions(arial)
                {
                    HorizontalAlignment = horizontalAlignment
                },
                staticItem.Caption, 
                Brushes.Solid(textColor),
                null
            );
        }

        return ctx;
    }
    
    public static IImageProcessingContext DrawTextButton(this IImageProcessingContext ctx, Item textButtonItem)
    {
        RectangleF? textRect = DimensionHelper.ParseRectangle(textButtonItem.Rectangle);

        if (textRect != null && textButtonItem.Caption != null)
        {
            Parameter? colorParam = textButtonItem.GetParameter("color");
            Color textColor;
            HorizontalAlignment horizontalAlignment;
            
            if (colorParam != null)
            {
                string staticColor = ColorHelper.ARGBtoRBGA(colorParam.Value);
                textColor = Color.Parse(staticColor);
            }
            else
            {
                textColor = Color.Black;
            }
            
            // TODO: Either pick a font closely referencing Pangya's UI
            //       Or add support for laoding in WFT font files
            FontFamily arialFontFamily = SystemFonts.Get("Malgun Gothic");
            Font arial = arialFontFamily.CreateFont(14f, FontStyle.Regular);
            
            ctx.DrawText(
                new DrawingOptions()
                {
                    Transform = Matrix3x2.CreateTranslation(textRect.Value.X, textRect.Value.Y)
                },
                new RichTextOptions(arial)
                {
                    HorizontalAlignment = HorizontalAlignment.Left
                },
                textButtonItem.Caption, 
                Brushes.Solid(textColor),
                null
            );
        }

        return ctx;
    }

    public static IImageProcessingContext DrawBase(this IImageProcessingContext ctx, Base baseElement, FileAtlas fileAtlas)
    {
        if (baseElement.Background != "")
        {
            Image? baseBackground = fileAtlas.GetImage(baseElement.Background);

            if (baseBackground != null)
            {
                ctx.DrawImage(baseBackground, new Point(0, 0), 1f);
            } 
        }

        return ctx;
    }

    public static IImageProcessingContext DrawInput(this IImageProcessingContext ctx, Item inputItem)
    {
        RectangleF? inputRect = DimensionHelper.ParseRectangle(inputItem.Rectangle);

        Parameter? backgroundColorParam = inputItem.GetParameter("bgcolor");
        Parameter? borderColorParam = inputItem.GetParameter("bordercolor");

        if (backgroundColorParam != null)
        {
            Color backgroundColor = Color.Parse(ColorHelper.ARGBtoRBGA(backgroundColorParam.Value));

            ctx.Fill(backgroundColor, (RectangleF)inputRect);
        }

        if (borderColorParam != null)
        {
            Color borderColor = Color.Parse(ColorHelper.ARGBtoRBGA(borderColorParam.Value));

            ctx.Draw(borderColor, 1f, (RectangleF)inputRect);
        }

        return ctx;
    }

    public static IImageProcessingContext DrawGaugebar(this IImageProcessingContext ctx, Item gaugebarItem)
    {
        RectangleF? gaugebarRect = DimensionHelper.ParseRectangle(gaugebarItem.Rectangle);
        string backgroundColor = ColorHelper.ARGBtoRBGA(gaugebarItem.GetParameter("bgcolor").Value);
        Color bgColor = Color.Parse(backgroundColor);
        string foregroundColor = ColorHelper.ARGBtoRBGA(gaugebarItem.GetParameter("color").Value);
        Color color = Color.Parse(foregroundColor);

        int? borderThickness = null;
        Parameter? borderThicknessParam = gaugebarItem.GetParameter("border_thickness");

        if (borderThicknessParam != null)
        {
            borderThickness = Int32.Parse(borderThicknessParam.Value);
        }
        
        if (gaugebarRect != null)
        {
            ctx.Fill(bgColor, (RectangleF)gaugebarRect);

            RectangleF valueRect = (RectangleF)gaugebarRect;
            valueRect.Width = valueRect.Width / 2;

            ctx.Fill(color, valueRect);

            if (borderThickness != null && borderThickness > 0)
            {
                ctx.Draw(bgColor, (float)borderThickness, (RectangleF)gaugebarRect);
            }
        }
        
        return ctx;
    }

    public static IImageProcessingContext DrawViewer(this IImageProcessingContext ctx, Item viewerItem, FileAtlas fileAtlas)
    {
        RectangleF? viewerRect = DimensionHelper.ParseRectangle(viewerItem.Rectangle);
        Image? bgImage = fileAtlas.GetImage(viewerItem.GetParameter("bgimg")?.Value);
        Image? image = fileAtlas.GetImage(viewerItem.GetParameter("image")?.Value);

        if (viewerRect == null)
        {
            return ctx;
        }
        
        if (bgImage != null)
        {
            ctx.DrawImage(bgImage, new Point((int)viewerRect.Value.X, (int)viewerRect.Value.Y), 1f);
        }
        
        if (image != null)
        {
            ctx.DrawImage(image, new Point((int)viewerRect.Value.X, (int)viewerRect.Value.Y), 1f);
        }

        return ctx;
    }

    public static IImageProcessingContext DrawDebugHint(this IImageProcessingContext ctx, Item item, Color color, FileAtlas fileAtlas)
    {
        if (color == Color.Transparent)
        {
            return ctx;
        }
        
        RectangleF? bounds = null;
        
        if (item.Rectangle != null)
        {
            bounds = DimensionHelper.ParseRectangle(item.Rectangle);
        }

        if (item.Position != null)
        {
            int[] pos = DimensionHelper.TextToIntArray(item.Position);

            Parameter? bgParam = item.GetParameter("bgimg");
            Parameter? normalParam = item.GetParameter("normal");
            Rectangle? texBounds = null;

            if (bgParam != null && bgParam.Value.Length > 0)
            {
                Image? bgTexture = fileAtlas.GetImage($"{bgParam.Value}.");

                if (bgTexture != null)
                {
                    texBounds = bgTexture.Bounds;    
                }
            }
            

            if (normalParam != null && normalParam.Value.Length > 0)
            {
                Image? normalTexture = fileAtlas.GetImage($"{normalParam.Value}.");

                if (normalTexture != null)
                {
                    texBounds = normalTexture.Bounds;    
                }
            }

            if (texBounds != null)
            {
                bounds = new RectangleF(pos[0], pos[1], texBounds.Value.Width, texBounds.Value.Height);    
            }
        }

        if (bounds != null)
        {
            ctx.Draw(color, 1f, (RectangleF)bounds);

            if (item.Name != null && item.Name.Length > 0)
            {
                FontFamily arialFontFamily = SystemFonts.Get("Arial");
                Font arial = arialFontFamily.CreateFont(12f, FontStyle.Regular);

                FontRectangle fontRectangle = TextMeasurer.MeasureBounds(item.Name, new TextOptions(arial));

                ctx.Fill(color, new RectangleF(bounds.Value.X, bounds.Value.Y, fontRectangle.Width, fontRectangle.Height));
                ctx.DrawText(item.Name, arial, Color.White, new PointF(bounds.Value.X, bounds.Value.Y));    
            }
        }

        return ctx;
    }
}