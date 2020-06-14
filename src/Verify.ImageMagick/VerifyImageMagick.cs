using System.IO;
using System.Threading.Tasks;
using ImageMagick;

namespace VerifyTests
{
    public static partial class VerifyImageMagick
    {
        public static void Initialize()
        {
            RegisterPdfToPngConverter();
            RegisterComparers();
        }

        public static void RegisterPdfToPngConverter()
        {
            VerifierSettings.RegisterFileConverter(
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
            VerifierSettings.RegisterComparer(
                extension,
                (settings, received, verified) => Compare(threshold, metric, format, received, verified));
        }

        static Task<CompareResult> Compare(double threshold, ErrorMetric metric, MagickFormat format, Stream received, Stream verified)
        {
            using var img1 = new MagickImage(received, format);
            using var img2 = new MagickImage(verified, format);
            //https://imagemagick.org/script/command-line-options.php#metric
            var diff = img1.Compare(img2, metric);
            var compare = diff < threshold;
            if (compare)
            {
                return Task.FromResult(CompareResult.Equal);
            }

            return Task.FromResult(CompareResult.NotEqual($"diff < threshold. threshold: {threshold}, diff: {diff}"));
        }
    }
}