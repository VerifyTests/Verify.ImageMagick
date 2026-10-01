namespace VerifyTests;

public static partial class VerifyImageMagick
{
    static ConversionResult ConvertSvg(string? name, Stream stream, IReadOnlyDictionary<string, object> context)
    {
        var content = ReadNormalizedSvg(stream);
        if (!outputs.HasFlag(ImageMagickOutputs.Png))
        {
            return new(null, [new("svg", content, name)]);
        }

        using var svgStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var svg = ReadSvgStream(svgStream, context);

        var pngStream = new MemoryStream();
        svg.Write(pngStream, MagickFormat.Png);

        return new(
            null,
            [
                new("svg", content, name),
                new("png", pngStream, name)
            ]);
    }

    // Verify requires text snapshots to use \n line endings, so an svg authored or checked out
    // with \r\n (or \r) would otherwise produce an unacceptable verified file.
    static string ReadNormalizedSvg(Stream stream)
    {
        stream = WrapStream(stream);
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
        return reader
            .ReadToEnd()
            .Replace("\r\n", "\n")
            .Replace('\r', '\n');
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