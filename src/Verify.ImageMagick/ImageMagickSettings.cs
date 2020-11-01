using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ImageMagick;

namespace VerifyTests
{
    public static class ImageMagickSettings
    {
        public static void PagesToInclude(this VerifySettings settings, int count)
        {
            Guard.AgainstNull(settings, nameof(settings));
            settings.Context["ImageMagick.PagesToInclude"] = count;
        }

        internal static bool GetPagesToInclude(this IReadOnlyDictionary<string, object> context, [NotNullWhen(true)] out int? pages)
        {
            Guard.AgainstNull(context, nameof(context));
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
            Guard.AgainstNull(settings, nameof(settings));
            Guard.AgainstNull(magickReadSettings, nameof(magickReadSettings));
            settings.Context["ImageMagick.MagickReadSettings"] = magickReadSettings;
        }

        internal static MagickReadSettings MagickReadSettings(this IReadOnlyDictionary<string, object> context)
        {
            Guard.AgainstNull(context, nameof(context));
            if (context.TryGetValue("ImageMagick.MagickReadSettings", out var value))
            {
                return (MagickReadSettings) value;
            }

            return new MagickReadSettings
            {
                Density = new Density(100, 100)
            };
        }
    }
}