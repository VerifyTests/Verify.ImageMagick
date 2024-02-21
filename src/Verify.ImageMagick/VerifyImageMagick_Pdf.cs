using VerifyTestsImageMagick;

namespace VerifyTests;

public static partial class VerifyImageMagick
{
    static ConversionResult Convert(Stream stream, IReadOnlyDictionary<string, object> context, MagickFormat magickFormat)
    {
        var streams = new List<Stream>();
        var magickSettings = context.MagickReadSettings();
        magickSettings.Format = magickFormat;
        using var images = new MagickImageCollection();
        images.Read(stream, magickSettings);
        var count = images.Count;
        if (context.GetPagesToInclude(out var pagesToInclude))
        {
            count = Math.Min(count, (int) pagesToInclude);
        }

        for (var index = 0; index < count; index++)
        {
            var image = images[index];
            var memoryStream = new MemoryStream();
            image.Write(memoryStream, MagickFormat.Png);
            streams.Add(memoryStream);
        }

        return new(null, streams.Select(_ => new Target("png", _)));
    }
}