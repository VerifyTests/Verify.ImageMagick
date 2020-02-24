using System.Diagnostics.CodeAnalysis;

namespace Verify
{
    public static class ImageMagickSettings
    {
        public static void PagesToInclude(this VerifySettings settings, int count)
        {
            Guard.AgainstNull(settings, nameof(settings));
            settings.Data["ImageMagickPagesToInclude"] = count;
        }

        internal static bool GetPagesToInclude(this VerifySettings settings, [NotNullWhen(true)] out int? pages)
        {
            Guard.AgainstNull(settings, nameof(settings));
            if (settings.Data.TryGetValue("ImageMagickPagesToInclude", out var value))
            {
                pages = (int?) value;
                return true;
            }

            pages = null;
            return false;
        }
    }
}