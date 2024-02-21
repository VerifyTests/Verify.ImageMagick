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
        RegisterPdfToPngConverter();
        RegisterComparers();
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
            (received, verified, _) => Compares(threshold, metric, received, verified));
    }

    static Task<CompareResult> Compares(double threshold, ErrorMetric metric, string received, string verified)
    {
        var utf8 = Encoding.UTF8;
        using var receivedStream = new MemoryStream(utf8.GetBytes(received));
        using var verifiedStream = new MemoryStream(utf8.GetBytes(verified));
        using var receivedImage = new MagickImage(receivedStream);
        using var verifiedImage = new MagickImage(verifiedStream);
        return Compare(threshold, metric, receivedImage, verifiedImage);
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

    static Task<CompareResult> Compare(double threshold, ErrorMetric metric, MagickImage received, MagickImage verified)
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
}