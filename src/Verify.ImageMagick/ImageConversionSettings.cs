namespace VerifyTestsImageMagick;

public class ImageConversionSettings
{
    /// <summary>
    /// Gets or sets the background color that should be set for the converted image.
    /// </summary>
    public MagickColor? BackgroundColor { get; set; }

    internal void Apply(IMagickImage<ushort> image)
    {
        if (BackgroundColor != null)
        {
            image.BackgroundColor = BackgroundColor;
        }
    }

    internal MagickImage ApplyAndFlatten(IMagickImage<ushort> image)
    {
        var result = image;
        if (BackgroundColor != null)
        {
            var collection = new MagickImageCollection(new[]
            {
                image
            });
            result = collection.Flatten(BackgroundColor);
        }

        return new (result);

    }
}