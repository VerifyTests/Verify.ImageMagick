namespace VerifyTests;

public static partial class VerifyImageMagick
{
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
        VerifierSettings.RegisterStreamConverter(
            "svg",
            ConvertSvg);
        VerifierSettings.RegisterStreamConverter(
            "png",
            (name, stream, context) => ConvertImage(name, stream, context, "png", MagickFormat.Png));
        VerifierSettings.RegisterStreamConverter(
            "tiff",
            (name, stream, context) => ConvertImage(name, stream, context, "tiff", MagickFormat.Tiff));
        RegisterPdfToPngConverter();
    }

    static ConversionResult ConvertImage(string? name, Stream stream, IReadOnlyDictionary<string, object> context, string extension, MagickFormat format)
    {
        var background = context.Background();
        if (background == null)
        {
            return new(null, [new(extension, stream, name)]);
        }

        var image = new MagickImage(
            WrapStream(stream),
            new MagickReadSettings
            {
                BackgroundColor = background,
                Format = format
            });
        var flattened = Flatten(image, background);
        var imageStream = new MemoryStream();
        flattened.Write(imageStream);
        return new(null, [new(extension, imageStream, name)]);
    }

    static IMagickImage<ushort> Flatten(IMagickImage<ushort> image, MagickColor background)
    {
        var collection = new MagickImageCollection([image]);
        return collection.Flatten(background);
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

    internal static Task<CompareResult> Compare(double threshold, ErrorMetric metric, Stream received, Stream verified)
    {
        using var receivedImage = new MagickImage(received);
        using var verifiedImage = new MagickImage(verified);
        return Compare(threshold, metric, receivedImage, verifiedImage);
    }

    /// <summary>
    /// Helper method that calls <see cref="RegisterPdfToPngConverter"/> and
    /// <see cref="RegisterComparers"/>(threshold = .005, metric = ErrorMetric.Fuzz)
    /// </summary>
    public static void ImageMagickComparer(this VerifySettings settings, double threshold = .005, ErrorMetric metric = ErrorMetric.Fuzz) =>
        settings.UseStreamComparer(
            (received, verified, _) => Compare(threshold, metric, received, verified));

    /// <summary>
    /// Helper method that calls <see cref="RegisterPdfToPngConverter"/> and
    /// <see cref="RegisterComparers"/>(threshold = .005, metric = ErrorMetric.Fuzz)
    /// </summary>
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
                  * For one test: Verifier.VerifyFile("file.jpg").ImageMagickComparer({round});
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
