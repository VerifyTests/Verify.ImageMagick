namespace VerifyTests;

public static partial class VerifyImageMagick
{
    /// <summary>
    /// Helper method that calls <see cref="RegisterPdfToPngConverter"/> and
    /// <see cref="RegisterComparers"/>(threshold = .005, metric = ErrorMetric.Fuzz)
    /// </summary>
    public static bool Initialized { get; private set; }

    public static void Initialize()
    {
        if (Initialized)
        {
            throw new("Already Initialized");
        }

        Initialized = true;

        InnerVerifier.ThrowIfVerifyHasBeenRun();
        FileExtensions.RemoveTextExtension("svg");
        VerifierSettings.RegisterFileConverter(
            "svg",
            ConvertSvg);
        VerifierSettings.RegisterFileConverter(
            "png",
            (stream, context) => ConvertImage(stream, context, "png", MagickFormat.Png));
        VerifierSettings.RegisterFileConverter(
            "tiff",
            (stream, context) => ConvertImage(stream, context, "tiff", MagickFormat.Tiff));
        RegisterPdfToPngConverter();
    }

    static ConversionResult ConvertSvg(Stream stream, IReadOnlyDictionary<string, object> context)
    {
        var svg = ReadSvg(stream, context);
        var pngStream = new MemoryStream();
        svg.Write(pngStream, MagickFormat.Png);
        var targets = new List<Target>
        {
            new("svg", stream),
            new("png", pngStream)
        };

        return new(null, targets);
    }

    static ConversionResult ConvertImage(Stream stream, IReadOnlyDictionary<string, object> context, string extension, MagickFormat format)
    {
        var image = ReadImage(stream, context, format);
        var imageStream = new MemoryStream();
        image.Write(imageStream);
        return new(null, [new(extension, imageStream)]);
    }

    public static void RegisterPdfToPngConverter()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        VerifierSettings.RegisterFileConverter(
            "pdf",
            (stream, context) => Convert(stream, context, MagickFormat.Pdf));
    }

    public static void RegisterComparers(double threshold = .005, ErrorMetric metric = ErrorMetric.Fuzz)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        RegisterComparer(threshold, metric, "png");
        RegisterComparer(threshold, metric, "jpg");
        RegisterComparer(threshold, metric, "bmp");
        RegisterComparer(threshold, metric, "tiff");
        VerifierSettings.RegisterStringComparer(
            "svg",
            (received, verified, _) => CompareSvg(threshold, metric, received, verified));
    }

    static Task<CompareResult> CompareSvg(double threshold, ErrorMetric metric, string received, string verified)
    {
        var receivedImage = new MagickImage(Encoding.UTF8.GetBytes(received), MagickFormat.Svg);
        var verifiedImage = new MagickImage(Encoding.UTF8.GetBytes(verified), MagickFormat.Svg);
        return Compare(threshold, metric, receivedImage, verifiedImage);
    }

    static IMagickImage<ushort> ReadSvg(Stream stream, IReadOnlyDictionary<string, object> context) =>
        ReadImage(stream, context, MagickFormat.Svg);

    static IMagickImage<ushort> ReadImage(Stream stream, IReadOnlyDictionary<string, object> context, MagickFormat format)
    {
        stream = WrapStream(stream);

        var image = new MagickImage();
        var background = context.Background();
        if (background != null)
        {
            image.BackgroundColor = background;
        }

        image.Read(stream, format);
        return ApplyBackgroundColorAndFlatten(image, background);
    }

    static Stream WrapStream(Stream stream)
    {
        if (!stream.CanSeek)
        {
            var previousStream = stream;
            stream = new MemoryStream();
            previousStream.CopyTo(stream);
        }

        if (stream.Position != 0)
        {
            stream.Position = 0;
        }

        return stream;
    }

    static IMagickImage<ushort> ApplyBackgroundColorAndFlatten(IMagickImage<ushort> image, MagickColor? background)
    {
        if (background == null)
        {
            return new MagickImage(image);
        }

        var collection = new MagickImageCollection([image]);
        return new MagickImage(collection.Flatten(background));
    }

    internal static Task<CompareResult> Compare(double threshold, ErrorMetric metric, Stream received, Stream verified)
    {
        using var receivedImage = new MagickImage(received);
        using var verifiedImage = new MagickImage(verified);
        return Compare(threshold, metric, receivedImage, verifiedImage);
    }

    public static void ImageMagickComparer(this VerifySettings settings, double threshold = .005, ErrorMetric metric = ErrorMetric.Fuzz) =>
        settings.UseStreamComparer(
            (received, verified, _) => Compare(threshold, metric, received, verified));

    public static SettingsTask ImageMagickComparer(this SettingsTask settings, double threshold = .005, ErrorMetric metric = ErrorMetric.Fuzz) =>
        settings.UseStreamComparer(
            (received, verified, _) => Compare(threshold, metric, received, verified));

    public static void RegisterComparer(double threshold, ErrorMetric metric, string extension) =>
        VerifierSettings.RegisterStreamComparer(
            extension,
            (received, verified, _) => Compare(threshold, metric, received, verified));

    static Task<CompareResult> Compare(double threshold, ErrorMetric metric, IMagickImage<ushort> received, IMagickImage<ushort> verified)
    {
        //https://imagemagick.org/script/command-line-options.php#metric
        var diff = received.Compare(verified, metric);
        var compare = diff < threshold;
        if (compare)
        {
            return Task.FromResult(CompareResult.Equal);
        }

        var round = Math.Ceiling(diff * 100) / 100;
        return Task.FromResult(
            CompareResult.NotEqual(
                $"""
                 diff({diff}) > threshold({threshold}).
                 If this difference is acceptable, use:

                  * Globally: VerifyImageMagick.RegisterComparers({round});
                  * For one test: Verifier.VerifyFile("file.jpg").RegisterComparers({round});
                 """));
    }


    //
    // static ImageInfo BuildInfo(MagickImage image) =>
    //     new()
    //     {
    //         Height = image.Height,
    //         Width = image.Width,
    //         Gamma = image.Gamma,
    //         Depth = image.Depth,
    //         Orientation = image.Orientation,
    //         Label = image.Label,
    //         Quality = image.Quality,
    //         Comment = image.Comment,
    //         Compression = image.Compression,
    //         Density = image.Density.ToString(),
    //         AnimationDelay = image.AnimationDelay,
    //         AnimationIterations = image.AnimationIterations,
    //         BaseWidth = image.BaseWidth,
    //         BaseHeight = image.BaseHeight,
    //         BackgroundColor = image.BackgroundColor?.ToString(),
    //         BorderColor = image.BorderColor?.ToString(),
    //         ChannelCount = image.ChannelCount,
    //         ColorFuzz = image.ColorFuzz,
    //         ColormapSize = image.ColormapSize,
    //         ColorSpace = image.ColorSpace,
    //         ColorType = image.ColorType,
    //         HasAlpha = image.HasAlpha,
    //         IsOpaque = image.IsOpaque,
    //         MatteColor = image.MatteColor?.ToString(),
    //         BlackPointCompensation = image.BlackPointCompensation,
    //         AnimationTicksPerSecond = image.AnimationTicksPerSecond
    //     };
}