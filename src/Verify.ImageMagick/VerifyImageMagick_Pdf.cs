using System;
using System.Collections.Generic;
using System.IO;
using ImageMagick;
using Verify;

public static partial class VerifyImageMagick
{
    static ConversionResult ConvertPdf(Stream stream, VerifySettings verifySettings)
    {
        var streams = new List<Stream>();
        // Settings the density to 300 dpi will create an image with a better quality
        var magickSettings = new MagickReadSettings
        {
            Density = new Density(100, 100),
            Format = MagickFormat.Pdf
        };
        using var images = new MagickImageCollection();
        images.Read(stream, magickSettings);
        var count = images.Count;
        if (verifySettings.GetPagesToInclude(out var pagesToInclude))
        {
            count = Math.Min(count, (int)pagesToInclude);
        }

        for (var index = 0; index < count; index++)
        {
            var image = images[index];
            var memoryStream = new MemoryStream();
            image.Write(memoryStream, MagickFormat.Png);
            streams.Add(memoryStream);
        }

        return new ConversionResult(null, streams);
    }
}