using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ImageMagick;

namespace VerifyTests
{
    public static class ImageMagickSettings
    {
        public static void PagesToInclude(this VerifySettings settings, int count)
        {
            settings.Context["ImageMagick.PagesToInclude"] = count;
        }

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

        public static void MagickReadSettings(this VerifySettings settings, MagickReadSettings magickReadSettings)
        {
            settings.Context["ImageMagick.MagickReadSettings"] = magickReadSettings;
        }

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
    }
}