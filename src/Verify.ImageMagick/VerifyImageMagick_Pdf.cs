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

    internal static ConversionResult Convert(string? name, Stream stream, IReadOnlyDictionary<string, object> context, MagickFormat magickFormat)
    {
        var streams = new List<Stream>();
        var magickSettings = context.MagickReadSettings();
        magickSettings.Format = magickFormat;
        var password = context.PdfPassword();
        var includePdf = !context.IsTargetExcluded("pdf");
        if (password != null)
        {
            // Checked before rendering, which is the expensive part, so the failure is immediate.
            // An encrypted document cannot have a deterministic pdf snapshot: the trailer /ID seeds
            // the encryption key, so neutralizing it would leave the document undecryptable. Rather
            // than silently omitting the target, which would make the snapshot set differ from an
            // unencrypted document for no visible reason, this is explicit.
            if (includePdf)
            {
                throw new(
                    """
                    A password protected pdf cannot produce a deterministic pdf target, since the trailer /ID seeds the encryption key and neutralizing it would leave the document undecryptable.
                    Exclude the pdf target to verify only the rendered pages: ExcludeTargets("pdf")
                    """);
            }

            magickSettings.SetDefines(
                new PdfReadDefines
                {
                    Password = password
                });
        }

        // Made seekable so the source document can be re-read for the pdf target after
        // MagickImageCollection has consumed it.
        stream = WrapStream(stream);

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

        List<Target> targets = [];

        // The pdf snapshot is always the full document, regardless of PagesToInclude, which trims
        // only the rendered pages above. Mirrors the svg target in ConvertSvg, which likewise emits
        // the source document alongside the render.
        if (includePdf)
        {
            stream.Position = 0;
            var pdf = context.Normalize() ? PdfNormalizer.Normalize(stream) : CopyRemaining(stream);
            targets.Add(
                new("pdf", pdf, name, performConversion: false)
                {
                    BypassComparersForSubsequentOnDifference = true
                });
        }

        targets.AddRange(streams.Select(_ => new Target("png", _, name)));
        return new(null, targets);
    }

    // The source stream is consumed elsewhere in this method, so the pdf target gets its own copy.
    static MemoryStream CopyRemaining(Stream stream)
    {
        var target = new MemoryStream();
        stream.CopyTo(target);
        target.Position = 0;
        return target;
    }
}
