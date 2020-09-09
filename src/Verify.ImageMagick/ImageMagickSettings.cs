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

        internal static bool GetPagesToInclude(this VerifySettings settings, [NotNullWhen(true)] out int? pages)
        {
            Guard.AgainstNull(settings, nameof(settings));
            if (settings.Context.TryGetValue("ImageMagick.PagesToInclude", out var value))
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

        internal static MagickReadSettings MagickReadSettings(this VerifySettings settings)
        {
            Guard.AgainstNull(settings, nameof(settings));
            if (settings.Context.TryGetValue("ImageMagick.MagickReadSettings", out var value))
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