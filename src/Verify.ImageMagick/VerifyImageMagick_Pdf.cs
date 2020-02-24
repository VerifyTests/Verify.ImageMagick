using System.Collections.Generic;
using System.IO;
using ImageMagick;
using Verify;

public static partial class VerifyImageMagick
{
    static ConversionResult ConvertPdf(Stream stream, VerifySettings verifySettings)
    {
        // Settings the density to 300 dpi will create an image with a better quality
        var magickSettings = new MagickReadSettings
        {
            Density = new Density(100, 100),
            Format = MagickFormat.Pdf,
            //BackgroundColor = MagickColor.FromRgb(255,255,255)
        };

        using var images = new MagickImageCollection();
        // Add all the pages of the pdf file to the collection
        images.Read(stream, magickSettings);

        var streams = new List<Stream>();
        foreach (var image in images)
        {
            var memoryStream = new MemoryStream();
            image.Write(memoryStream, MagickFormat.Png);
            streams.Add(memoryStream);
        }

        return new ConversionResult(null, streams);
    }
}