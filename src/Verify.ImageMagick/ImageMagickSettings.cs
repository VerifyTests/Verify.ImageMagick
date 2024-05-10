namespace VerifyTestsImageMagick;

public static class ImageMagickSettings
{
    static MagickColor? background;

    public static void ImageMagickBackground(MagickColor color) =>
        background = color;

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
}