namespace VerifyTests;

public static partial class VerifyImageMagick
{
    public static void RegisterPdfToPngConverter()
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        VerifierSettings.RegisterStreamConverter(
            "pdf",
            (_, stream, context) => Convert(stream, context, MagickFormat.Pdf));
    }

    internal static ConversionResult Convert(Stream stream, IReadOnlyDictionary<string, object> context, MagickFormat magickFormat)
    {
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

        // Names the pages and says which of them the verification wants, so that is decided the
        // same way here as in every other converter of a paged document.
        var conversion = new PagedConversion(context);

        // Ghostscript draws every page in the one read, so the read can only be skipped whole:
        // when the pngs are excluded. The pages are then only counted, which reads what each
        // one is without drawing it, so the info file says how many there are either way.
        if (conversion.IncludeImages)
        {
            using var images = new MagickImageCollection();
            images.Read(stream, magickSettings);
            var background = context.Background();
            foreach (var number in conversion.Pages(images.Count))
            {
                var image = images[number - 1];
                if (background != null)
                {
                    image = Flatten(image, background);
                }

                var memoryStream = new MemoryStream();
                image.Write(memoryStream, MagickFormat.Png);
                conversion.AddPage(number, memoryStream);
            }
        }
        else
        {
            using var images = new MagickImageCollection();
            images.Ping(stream, magickSettings);
            conversion.PageCount = images.Count;
        }

        // The pdf snapshot is always the full document, regardless of PagesToInclude, which trims
        // only the rendered pages above. Mirrors the svg target in ConvertSvg, which likewise emits
        // the source document alongside the render.
        if (includePdf)
        {
            stream.Position = 0;
            var pdf = context.Normalize() ? PdfNormalizer.Normalize(stream) : CopyRemaining(stream);
            conversion.Source(new("pdf", pdf));
        }

        return conversion.Build();
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
