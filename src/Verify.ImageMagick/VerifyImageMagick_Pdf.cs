using System.Collections.Generic;
using System.IO;
using System.Threading;
using ImageMagick;
using Verify;

public static partial class VerifyImageMagick
{
    static SynchronizationContext scheduler = new SynchronizationContext();

    static ConversionResult ConvertPdf(Stream stream, VerifySettings verifySettings)
    {
        var streams = new List<Stream>();
        scheduler.Send(state =>
        {
            // Settings the density to 300 dpi will create an image with a better quality
            var magickSettings = new MagickReadSettings
            {
                Density = new Density(100, 100),
                Format = MagickFormat.Pdf
            };
            using var images = new MagickImageCollection();
            // Add all the pages of the pdf file to the collection
            images.Read(stream, magickSettings);

            foreach (var image in images)
            {
                var memoryStream = new MemoryStream();
                image.Write(memoryStream, MagickFormat.Png);
                streams.Add(memoryStream);
            }
        }, null);

        return new ConversionResult(null, streams);
    }
}