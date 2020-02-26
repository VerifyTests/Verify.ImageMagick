using System.IO;
using ImageMagick;
using Verify;

public static partial class VerifyImageMagick
{
    public static void Initialize()
    {
        RegisterPdfToPngConverter();
        RegisterComparers();
    }

    public static void RegisterPdfToPngConverter()
    {
        SharedVerifySettings.RegisterFileConverter(
            "pdf",
            "png",
            (stream, settings) => Convert(stream, settings, MagickFormat.Pdf));
    }

    public static void RegisterComparers(double threshold = .005, ErrorMetric metric = ErrorMetric.Fuzz)
    {
        RegisterComparer(threshold, metric, "png", MagickFormat.Png);
        RegisterComparer(threshold, metric, "jpg", MagickFormat.Jpg);
        RegisterComparer(threshold, metric, "bmp", MagickFormat.Bmp);
        RegisterComparer(threshold, metric, "tiff", MagickFormat.Tiff);
    }

    public static void RegisterComparer(double threshold, ErrorMetric metric, string extension, MagickFormat format)
    {
        SharedVerifySettings.RegisterComparer(
            extension,
            (stream1, stream2) => Compare(threshold, metric, format, stream1, stream2));
    }

    static bool Compare(double threshold, ErrorMetric metric, MagickFormat format, Stream stream1,  Stream stream2)
    {
        using var img1 = new MagickImage(stream1, format);
        using var img2 = new MagickImage(stream2, format);
        //https://imagemagick.org/script/command-line-options.php#metric
        var diff = img1.Compare(img2, metric);
        return diff < threshold;
    }
}