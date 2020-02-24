using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ImageMagick;
using Verify;

public static partial class VerifyImageMagick
{
    static ConcurrentExclusiveSchedulerPair scheduler = new ConcurrentExclusiveSchedulerPair();

    static async Task<ConversionResult> ConvertPdf(Stream stream, VerifySettings verifySettings)
    {
        var streams = new List<Stream>();

        await Task.Factory.StartNew(() =>
            {
                // Settings the density to 300 dpi will create an image with a better quality
                var magickSettings = new MagickReadSettings
                {
                    Density = new Density(100, 100),
                    Format = MagickFormat.Pdf,
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

            },
            CancellationToken.None,
            TaskCreationOptions.DenyChildAttach,
            scheduler.ExclusiveScheduler);

        return new ConversionResult(null, streams);
    }
}