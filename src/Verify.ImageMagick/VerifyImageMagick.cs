using ImageMagick;
using Verify;

public static partial class VerifyImageMagick
{
    public static void Initialize()
    {
        SharedVerifySettings.RegisterFileConverter("pdf", "png", ConvertPdf);
        SharedVerifySettings.RegisterComparer("png", (stream1, stream2) =>
        {
            using var img1 = new MagickImage(stream1);
            using var img2 = new MagickImage(stream2);
            //https://imagemagick.org/script/command-line-options.php#metric
            var diff = img1.Compare(img2, ErrorMetric.Fuzz);
            return diff < .0001;
        });
    }
}