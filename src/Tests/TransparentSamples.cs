#if DEBUG

[TestFixture]
public class TransparentSamples
{
    [Test]
    public Task TransparentSample(
        [Values("png", "svg", "pdf")] string format,
        [Values] Color backgroundColor) =>
        VerifyFile($"transparent.{format}")
            .ImageConversionSettings(
                new()
                {
                    BackgroundColor = Map(backgroundColor)
                });

    static MagickColor? Map(Color color) => color switch
    {
        Color.None => null,
        Color.Transparent => MagickColors.Transparent,
        Color.Green => MagickColors.Green,
        Color.Blue => MagickColors.Blue,
        _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
    };

    public enum Color
    {
        None,
        Transparent,
        Green,
        Blue
    }
}

#endif