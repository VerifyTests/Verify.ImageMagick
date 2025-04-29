namespace VerifyTests;

public static partial class VerifyImageMagick
{
    public static void RegisterPdfToPngConverter()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        VerifierSettings.RegisterStreamConverter(
            "pdf",
            (name, stream, context) => Convert(name, stream, context, MagickFormat.Pdf));
    }

    static ConversionResult Convert(string? name, Stream stream, IReadOnlyDictionary<string, object> context, MagickFormat magickFormat)
    {
        var streams = new List<Stream>();
        var magickSettings = context.MagickReadSettings();
        magickSettings.Format = magickFormat;
        var password = context.PdfPassword();
        if (password != null)
        {
            magickSettings.SetDefines(
                new PdfReadDefines
                {
                    Password = password
                });
        }

        using var images = new MagickImageCollection();
        images.Read(stream, magickSettings);
        var count = images.Count;
        if (context.GetPagesToInclude(out var pagesToInclude))
        {
            count = Math.Min(count, (int) pagesToInclude);
        }

        var background = context.Background();
        for (var index = 0; index < count; index++)
        {
            var image = images[index];
            if (background != null)
            {
                image = Flatten(image, background);
            }

            var memoryStream = new MemoryStream();
            image.Write(memoryStream, MagickFormat.Png);
            streams.Add(memoryStream);
        }

        return new(null, streams.Select(_ => new Target("png", _, name)));
    }
}