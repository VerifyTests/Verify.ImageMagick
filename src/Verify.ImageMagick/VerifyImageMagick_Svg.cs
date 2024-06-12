namespace VerifyTests;

public static partial class VerifyImageMagick
{
    static ConversionResult ConvertSvg(Stream stream, IReadOnlyDictionary<string, object> context)
    {
        stream = WrapStream(stream);
        using var svg = ReadSvgStream(stream, context);

        var pngStream = new MemoryStream();
        svg.Write(pngStream, MagickFormat.Png);

        return new(
            null,
            new List<Target>
            {
                new("svg", stream),
                new("png", pngStream)
            });
    }

    static IMagickImage<ushort> ReadSvgStream(Stream stream, IReadOnlyDictionary<string, object> context)
    {
        var background = context.Background();
        if (background == null)
        {
            return new MagickImage(stream, MagickFormat.Svg);
        }

        using var image = new MagickImage(
            stream,
            new MagickReadSettings
            {
                BackgroundColor = background,
                Format = MagickFormat.Svg
            });
        return Flatten(image, background);
    }

    static Task<CompareResult> CompareSvg(double threshold, ErrorMetric metric, string received, string verified)
    {
        using var receivedImage = ReadSvgString(received);
        using var verifiedImage = ReadSvgString(verified);
        return Compare(threshold, metric, receivedImage, verifiedImage);
    }

    static MagickImage ReadSvgString(string content) =>
        new(Encoding.UTF8.GetBytes(content), MagickFormat.Svg);
}