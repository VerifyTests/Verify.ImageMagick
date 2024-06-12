namespace VerifyTests;

public static partial class VerifyImageMagick
{
    static ConversionResult ConvertSvg(Stream stream, IReadOnlyDictionary<string, object> context)
    {
        stream = WrapStream(stream);
        var background = context.Background();
        if (background == null)
        {
            var svg = new MagickImage(stream, MagickFormat.Svg);
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
        else
        {
            var image = new MagickImage(
                stream,
                new MagickReadSettings
                {
                    BackgroundColor = background,
                    Format = MagickFormat.Svg
                });
            var svg = Flatten(image, background);
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
    }

    static Task<CompareResult> CompareSvg(double threshold, ErrorMetric metric, string received, string verified)
    {
        using var receivedImage = ReadSvg(received);
        using var verifiedImage = ReadSvg(verified);
        return Compare(threshold, metric, receivedImage, verifiedImage);
    }

    static MagickImage ReadSvg(string content) =>
        new(Encoding.UTF8.GetBytes(content), MagickFormat.Svg);
}