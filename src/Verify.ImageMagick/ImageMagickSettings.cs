namespace VerifyTestsImageMagick;

public static class ImageMagickSettings
{
    static ImageConversionSettings imageConversionSettings = new();

    public static void UseImageConversionSettings(ImageConversionSettings settings) =>
        imageConversionSettings = settings;

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

    public static void ImageConversionSettings(this VerifySettings settings, ImageConversionSettings magickReadSettings) =>
        settings.Context["ImageMagick.ImageConversionSettings"] = magickReadSettings;

    public static SettingsTask ImageConversionSettings(this SettingsTask settings, ImageConversionSettings magickReadSettings)
    {
        settings.CurrentSettings.ImageConversionSettings(magickReadSettings);
        return settings;
    }

    internal static ImageConversionSettings ImageConversionSettings(this IReadOnlyDictionary<string, object> context)
    {
        if (context.TryGetValue("ImageMagick.ImageConversionSettings", out var value))
        {
            return (ImageConversionSettings) value;
        }

        return imageConversionSettings;
    }
}