namespace VerifyTestsImageMagick;

public static class ImageMagickSettings
{
    static MagickColor? background;
    static string? pdfPassword;

    public static void ImageMagickBackground(MagickColor color) =>
        background = color;

    public static void ImageMagickPdfPassword(string password) =>
        pdfPassword = password;

    public static void PagesToInclude(this VerifySettings settings, int count) =>
        settings.Context["ImageMagick.PagesToInclude"] = count;

    public static SettingsTask PagesToInclude(this SettingsTask settings, int count)
    {
        settings.CurrentSettings.PagesToInclude(count);
        return settings;
    }

    internal static bool GetPagesToInclude(this IReadOnlyDictionary<string, object> context, [NotNullWhen(true)] out int? pages)
    {
        if (context.TryGetValue("ImageMagick.PagesToInclude", out var value))
        {
            pages = (int) value;
            return true;
        }

        pages = null;
        return false;
    }

    public static void MagickReadSettings(this VerifySettings settings, MagickReadSettings magickReadSettings) =>
        settings.Context["ImageMagick.MagickReadSettings"] = magickReadSettings;

    public static SettingsTask MagickReadSettings(this SettingsTask settings, MagickReadSettings magickReadSettings)
    {
        settings.CurrentSettings.MagickReadSettings(magickReadSettings);
        return settings;
    }

    internal static MagickReadSettings MagickReadSettings(this IReadOnlyDictionary<string, object> context)
    {
        if (context.TryGetValue("ImageMagick.MagickReadSettings", out var value))
        {
            return (MagickReadSettings) value;
        }

        return new()
        {
            Density = new(100, 100)
        };
    }

    public static void ImageMagickBackground(this VerifySettings settings, MagickColor color) =>
        settings.Context["ImageMagick.Background"] = color;

    public static SettingsTask ImageMagickBackground(this SettingsTask settings, MagickColor color)
    {
        settings.CurrentSettings.ImageMagickBackground(color);
        return settings;
    }

    internal static MagickColor? Background(this IReadOnlyDictionary<string, object> context)
    {
        if (context.TryGetValue("ImageMagick.Background", out var value))
        {
            return (MagickColor?) value;
        }

        return background;
    }


    public static void ImageMagickPdfPassword(this VerifySettings settings, string password) =>
        settings.Context["ImageMagick.PdfPassword"] = password;

    public static SettingsTask ImageMagickPdfPassword(this SettingsTask settings, string password)
    {
        settings.CurrentSettings.ImageMagickPdfPassword(password);
        return settings;
    }

    internal static string? PdfPassword(this IReadOnlyDictionary<string, object> context)
    {
        if (context.TryGetValue("ImageMagick.PdfPassword", out var value))
        {
            return (string?) value;
        }

        return pdfPassword;
    }

    /// <summary>
    /// Snapshots the pdf bytes exactly as produced, skipping the normalization that neutralizes the
    /// trailer <c>/ID</c>, the <c>/CreationDate</c> and <c>/ModDate</c>, and the XMP dates and
    /// identifiers. Use it when the producer already emits byte-deterministic documents, since
    /// normalizing them again copies the whole buffer, rescans it, and — when the XMP packet is
    /// canonicalized — rebuilds it and repairs the cross-reference table, all to change nothing.
    /// </summary>
    /// <remarks>
    /// Only skip this when the producer is genuinely deterministic. Without it a freshly generated
    /// pdf carries a wall-clock <c>/CreationDate</c> and a fresh <c>/ID</c>, so the snapshot differs
    /// on every run.
    /// <para>
    /// The XMP canonicalization is worth calling out because it is the pass that changes bytes for
    /// an already-deterministic producer: it collapses the packet's whitespace, so enabling or
    /// disabling this setting on an existing suite shifts the stored <c>.verified.pdf</c> even
    /// though nothing about the document changed. Expect to re-accept those snapshots once.
    /// </para>
    /// </remarks>
    public static void SkipPdfNormalization(this VerifySettings settings) =>
        settings.Context["ImageMagick.SkipNormalization"] = true;

    /// <inheritdoc cref="SkipPdfNormalization(VerifySettings)"/>
    public static SettingsTask SkipPdfNormalization(this SettingsTask settings)
    {
        settings.CurrentSettings.SkipPdfNormalization();
        return settings;
    }

    internal static bool Normalize(this IReadOnlyDictionary<string, object> context) =>
        !context.TryGetValue("ImageMagick.SkipNormalization", out var value) ||
        value is not true;
}
